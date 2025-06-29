using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstaFandy.DAL.Entities;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.DAL.Repos
{
    public class ChatRepo : GeneralRepo<Chat>, IChatRepo
    {
        private readonly AppDbContext _context;

        public ChatRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Chat? GetByBookingId(int bookingId)
        {
            return _context.Chats.FirstOrDefault(c => c.BookingId == bookingId);
        }
    }


}
