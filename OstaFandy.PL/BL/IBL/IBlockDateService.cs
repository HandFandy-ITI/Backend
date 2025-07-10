using Microsoft.AspNetCore.Http.HttpResults;
using System;
using OstaFandy.DAL.Entities;
using OstaFandy.PL.Constants;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IBlockDateService
    {
        PaginationHelper<BlockDateDTO> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, string? status = null, DateTime? Date = null);
        PaginationHelper<HandymanSummaryDTO> GetAllHandymanData(string searchString = "", int pageNumber = 1, int pageSize = 5, int? categoryId = null);

        public List<Category> GetAllCategory();

            Task<bool> AddBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate);
        public bool RejectBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate);
        public bool ApproveBlockDate(int HandymanId, string Reason, DateOnly StartDate, DateOnly EndDate);

    }

}
