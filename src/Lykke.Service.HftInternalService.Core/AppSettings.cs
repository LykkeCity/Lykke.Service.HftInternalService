﻿namespace Lykke.Service.HftInternalService.Core
{
    public class AppSettings
    {
        public HftInternalServiceSettings HftInternalServiceService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class HftInternalServiceSettings
    {
        public DbSettings Db { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
