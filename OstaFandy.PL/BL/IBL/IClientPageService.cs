namespace OstaFandy.PL.BL.IBL
{
    public interface IClientPageService
    {
        bool ApproveJobStatusChange(int jobId, string approvedStatus, int clientUserId);
        bool ApproveQuoteStatusChange(int jobId, string approvedStatus, int clientUserId);
    }
}
