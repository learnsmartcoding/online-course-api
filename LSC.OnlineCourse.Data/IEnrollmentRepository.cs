using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment);
        Task<Enrollment> GetEnrollmentByIdAsync(int id);
        Task<List<Enrollment>> GetEnrollmentByUserIdAsync(int userId);
    }

    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly OnlineCourseDbContext _context;

        public EnrollmentRepository(OnlineCourseDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<Enrollment> GetEnrollmentByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Payments).Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);
        }

        public async Task<List<Enrollment>> GetEnrollmentByUserIdAsync(int userId)
        {
            return await _context.Enrollments
                .Include(e => e.Payments)
                .Include(e=>e.Course)
                .Where(e => e.UserId == userId).ToListAsync();
        }
    }


}
