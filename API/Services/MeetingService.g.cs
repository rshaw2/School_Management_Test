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
    /// The meetingService responsible for managing meeting related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting meeting information.
    /// </remarks>
    public interface IMeetingService
    {
        /// <summary>Retrieves a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <returns>The meeting data</returns>
        Meeting GetById(Guid id);

        /// <summary>Retrieves a list of meetings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of meetings</returns>
        List<Meeting> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new meeting</summary>
        /// <param name="model">The meeting data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Meeting model);

        /// <summary>Updates a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <param name="updatedEntity">The meeting data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Meeting updatedEntity);

        /// <summary>Updates a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <param name="updatedEntity">The meeting data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Meeting> updatedEntity);

        /// <summary>Deletes a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The meetingService responsible for managing meeting related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting meeting information.
    /// </remarks>
    public class MeetingService : IMeetingService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Meeting class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MeetingService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <returns>The meeting data</returns>
        public Meeting GetById(Guid id)
        {
            var entityData = _dbContext.Meeting.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of meetings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of meetings</returns>/// <exception cref="Exception"></exception>
        public List<Meeting> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMeeting(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new meeting</summary>
        /// <param name="model">The meeting data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Meeting model)
        {
            model.Id = CreateMeeting(model);
            return model.Id;
        }

        /// <summary>Updates a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <param name="updatedEntity">The meeting data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Meeting updatedEntity)
        {
            UpdateMeeting(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <param name="updatedEntity">The meeting data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Meeting> updatedEntity)
        {
            PatchMeeting(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific meeting by its primary key</summary>
        /// <param name="id">The primary key of the meeting</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMeeting(id);
            return true;
        }
        #region
        private List<Meeting> GetMeeting(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Meeting.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Meeting>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Meeting), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Meeting, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateMeeting(Meeting model)
        {
            _dbContext.Meeting.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMeeting(Guid id, Meeting updatedEntity)
        {
            _dbContext.Meeting.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMeeting(Guid id)
        {
            var entityData = _dbContext.Meeting.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Meeting.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMeeting(Guid id, JsonPatchDocument<Meeting> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Meeting.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Meeting.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}