﻿namespace Backend.Models
{
    public class IsAliveResponse
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Env { get; set; }
        public bool IsDebug { get; set; }
    }
}
