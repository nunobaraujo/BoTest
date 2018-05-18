using Core.Encryption;
using Core.Extensions;
using Core.Json;
using Core.Repositories;
using Core.Services.Session;
using Core.Services.Session.Models;
using Core.Settings;
using NBsoft.Logs.Interfaces;
using System;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class SessionService : ISessionService
    {
        public const int SessionMaxAge = 86400000;     // 1 day = 86400000 milliseconds
        public const int SessionTimeout = 900000;      // 15 minutes = 900000 milliseconds
        private const string Salt = "AA3B25E5D1CF";
        private const string IV = "FA8C5137F505310F";

        private readonly BackendSetting _settings;
        private readonly ISessionRepository _sessionrepository;
        private readonly ILogger _log;
        
        public SessionService(BackendSetting settings, ISessionRepository sessionrepository, ILogger log)
        {
            _settings = settings;
            _sessionrepository = sessionrepository;
            _log = log;
        }

        public async Task<string> CreateNewSession(string userId, string userInfo)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Value cannot be empty", nameof(userId));

            try
            {  
                DateTime sessionStart = DateTime.UtcNow;
                var newSession = new UserSession
                {
                    UserId = userId,
                    UserInfo = userInfo,
                    Registered = sessionStart,
                    LastAction = sessionStart
                };
                newSession.SessionToken = GenerateToken(newSession);
                
                await _sessionrepository.Session.New(newSession);
                await _log.WriteInfoAsync(nameof(SessionService), nameof(CreateNewSession), newSession.ToJson(), "New session created");
                return newSession.SessionToken;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(SessionService), nameof(CreateNewSession), null, ex);
                throw;
            }
        }
               
        public async Task<string> GetUserId(string sessionToken)
        {
            if (string.IsNullOrEmpty(sessionToken))
                throw new ArgumentException("Value cannot be empty", nameof(sessionToken));
            try
            {
                var session = await ValidateToken(sessionToken);
                if (session != null)
                {
                    if (IsValidSession(session))
                    {
                        var editableSession = session.ToDto();
                        editableSession.LastAction = DateTime.UtcNow;
                        await _sessionrepository.Session.Update(editableSession);
                        return session.UserId;
                    }
                    else
                        await _sessionrepository.Session.Remove(sessionToken);
                }
                return null;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(SessionService), nameof(GetUserId), null, ex);
                throw;
            }
        }

        public async Task KillSession(string sessionToken)
        {
            if (string.IsNullOrEmpty(sessionToken))
                throw new ArgumentException("Value cannot be empty", nameof(sessionToken));
            try
            {
                var session = await ValidateToken(sessionToken);
                if (session != null)
                {
                    await _sessionrepository.Session.Remove(sessionToken);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(SessionService), nameof(GetUserId), null, ex);
                throw;
            }
        }

        private string GenerateToken(IUserSession newSession)
        {
            string plain = $"{_settings.ApiKey};{newSession.UserId};{newSession.UserInfo};{newSession.Registered.ToString("u")}";
            return RijndaelSimple.Encrypt(plain, _settings.ApiKey, Salt, "SHA1", 1, IV, 128);
        }
        private async Task<IUserSession> ValidateToken(string sessionToken)
        {
            if (string.IsNullOrEmpty(sessionToken))
                return null;
            var plainToken = RijndaelSimple.Decrypt(sessionToken, _settings.ApiKey, Salt, "SHA1", 1, IV, 128);
            if (!plainToken.StartsWith(_settings.ApiKey))
            {
                await _log.WriteInfoAsync(nameof(SessionService), nameof(ValidateToken), sessionToken, "Invalid token");
                return null;
            }
            var session = await _sessionrepository.Session.Get(sessionToken);
            if (session != null && sessionToken != GenerateToken(session))
            {
                await _log.WriteInfoAsync(nameof(SessionService), nameof(ValidateToken), sessionToken, "Invalid token");
                return null;
            }
            return session;
        }
        private bool IsValidSession(IUserSession session)
        {
            if (session == null)
                return false;

            // Session expiration time hit
            if (DateTime.UtcNow > session.Registered.AddMilliseconds(SessionMaxAge))
            {
                _log.WriteInfoAsync(nameof(SessionService), nameof(IsValidSession), session.ToJson(), "Session expired");
                return false;
            }

            // Session timeout
            if (DateTime.UtcNow > session.LastAction.AddMilliseconds(SessionTimeout))
            {
                _log.WriteInfoAsync(nameof(SessionService), nameof(IsValidSession), session.ToJson(), "Session timeout");
                return false;
            }
            return true;

        }
                
    }
    
}
