using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public interface IReviewService
    {
        Task<ReviewResponseDTO> CreateReviewAsync(CreateReviewDTO createReviewDTO);
    }
}