using SchoolManagementTest.Models;
using SchoolManagementTest.Data;
using SchoolManagementTest.Filter;
using SchoolManagementTest.Entities;
using SchoolManagementTest.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace SchoolManagementTest.Services
{
    /// <summary>
    /// The attendanceService responsible for managing attendance related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attendance information.
    /// </remarks>
    public interface IAttendanceService
    {
        /// <summary>Retrieves a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <returns>The attendance data</returns>
        Attendance GetById(Guid id);

        /// <summary>Retrieves a list of attendances based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attendances</returns>
        List<Attendance> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new attendance</summary>
        /// <param name="model">The attendance data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Attendance model);

        /// <summary>Updates a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <param name="updatedEntity">The attendance data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Attendance updatedEntity);

        /// <summary>Updates a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <param name="updatedEntity">The attendance data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Attendance> updatedEntity);

        /// <summary>Deletes a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The attendanceService responsible for managing attendance related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting attendance information.
    /// </remarks>
    public class AttendanceService : IAttendanceService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Attendance class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public AttendanceService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <returns>The attendance data</returns>
        public Attendance GetById(Guid id)
        {
            var entityData = _dbContext.Attendance.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of attendances based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of attendances</returns>/// <exception cref="Exception"></exception>
        public List<Attendance> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetAttendance(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new attendance</summary>
        /// <param name="model">The attendance data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Attendance model)
        {
            model.Id = CreateAttendance(model);
            return model.Id;
        }

        /// <summary>Updates a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <param name="updatedEntity">The attendance data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Attendance updatedEntity)
        {
            UpdateAttendance(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <param name="updatedEntity">The attendance data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Attendance> updatedEntity)
        {
            PatchAttendance(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific attendance by its primary key</summary>
        /// <param name="id">The primary key of the attendance</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteAttendance(id);
            return true;
        }
        #region
        private List<Attendance> GetAttendance(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Attendance.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Attendance>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Attendance), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Attendance, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateAttendance(Attendance model)
        {
            _dbContext.Attendance.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateAttendance(Guid id, Attendance updatedEntity)
        {
            _dbContext.Attendance.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteAttendance(Guid id)
        {
            var entityData = _dbContext.Attendance.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Attendance.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchAttendance(Guid id, JsonPatchDocument<Attendance> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Attendance.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Attendance.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}