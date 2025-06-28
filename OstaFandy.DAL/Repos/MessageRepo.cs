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
    public class MessageRepo : GeneralRepo<Message>, IMessageRepo
    {
        private readonly AppDbContext _context;

        public MessageRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Message> GetByChatId(int chatId)
        {
            return _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToList();
        }
    }


}
