﻿using System;

namespace Contracts.Requests
{
    public class DateIntervalRequest : BearerTokenRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}