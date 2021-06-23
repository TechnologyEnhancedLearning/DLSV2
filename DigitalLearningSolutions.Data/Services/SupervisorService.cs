namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Data.Models.Email;
    public interface ISupervisorService
    {
        //GET DATA

        //UPDATE DATA

        //INSERT DATA

        //DELETE DATA


    }
    public class SupervisorService : ISupervisorService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorService> logger;
        public SupervisorService(IDbConnection connection, ILogger<SupervisorService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
    }
}
