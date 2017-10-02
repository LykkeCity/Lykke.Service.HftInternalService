﻿using System;
using Common.Log;

namespace Lykke.Service.HftInternalService.Client
{
    public class HftInternalServiceClient : IHftInternalServiceClient, IDisposable
    {
        private readonly ILog _log;

        public HftInternalServiceClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
