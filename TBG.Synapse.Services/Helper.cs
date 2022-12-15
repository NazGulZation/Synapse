using System.Diagnostics;
using System.Text;
using TBG.Synapse.Repository;

namespace TBG.Synapse.Services
{
    public static class Helper
    {
        private readonly static SynapseRepository<LogError> logErrorRepo;

        static Helper()
        {
            logErrorRepo = new SynapseRepository<LogError>("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=Synapse;Integrated Security=True;");
            logErrorRepo.CreateTable();
        }

        public static string LogError(Exception ex)
        {
            LogError logError = GetLogError(ex);
            logError.ID = Guid.NewGuid().ToString();
            logError.TimeStamp = DateTime.Now;

            logErrorRepo.Insert(logError);

            return $"Error on system with ID {logError.ID}";
        }

        public static LogError GetLogError(Exception ex)
        {
            var logError = new LogError();
            logError.Message = ex.Message;
            StackTrace stackTrace = new StackTrace(ex, true);
            if (stackTrace.GetFrames().Count() > 0)
            {
                FileInfo fileInfo = new FileInfo(stackTrace.GetFrame(0).GetFileName());
                logError.FileName = stackTrace.GetFrame(0).GetFileName();
                logError.Line = stackTrace.GetFrame(0).GetFileLineNumber();
                logError.Column = stackTrace.GetFrame(0).GetFileColumnNumber();
                logError.Code = stackTrace.GetFrame(0).GetMethod().Name;
            }
            return logError;
        }
    }
}