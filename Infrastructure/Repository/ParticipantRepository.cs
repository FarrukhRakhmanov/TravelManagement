using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class ParticipantRepository : Repository<Participant>, IParticipantRepository
    {
        private readonly ApplicationDbContext _db;
        public ParticipantRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Participant participant)
        {
            _db.Participants.Update(participant);
        }
    }
}
