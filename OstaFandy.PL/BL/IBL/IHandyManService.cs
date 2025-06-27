﻿using OstaFandy.DAL.Entities;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.BL.IBL
{
    public interface IHandyManService
    {
        PaginationHelper<Handyman> GetAll(string searchString = "", int pageNumber = 1, int pageSize = 5, bool? isActive = null);


        List<AdminHandyManDTO> GetAllPendingHandymen();
        Task<bool> UpdateHandymanStatusById(int userId, string status);

 
        AdminHandyManDTO CreateHandyman(CreateHandymanDTO createHandymanDto);
        AdminHandyManDTO GetById(int id);
        bool DeleteHandyman(int id);
        AdminHandyManDTO EditHandyman(EditHandymanDTO editHandymanDto);

        public Task<int> CreateHandyManApplicationAsync(HandyManApplicationDto handymandto);

        HandyManStatsDto? GetHandyManStats(int handymanId);



    }
}

