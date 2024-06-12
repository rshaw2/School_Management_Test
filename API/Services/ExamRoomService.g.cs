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
    /// The examroomService responsible for managing examroom related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examroom information.
    /// </remarks>
    public interface IExamRoomService
    {
        /// <summary>Retrieves a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <returns>The examroom data</returns>
        ExamRoom GetById(Guid id);

        /// <summary>Retrieves a list of examrooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examrooms</returns>
        List<ExamRoom> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examroom</summary>
        /// <param name="model">The examroom data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ExamRoom model);

        /// <summary>Updates a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <param name="updatedEntity">The examroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ExamRoom updatedEntity);

        /// <summary>Updates a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <param name="updatedEntity">The examroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ExamRoom> updatedEntity);

        /// <summary>Deletes a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examroomService responsible for managing examroom related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examroom information.
    /// </remarks>
    public class ExamRoomService : IExamRoomService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ExamRoom class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExamRoomService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <returns>The examroom data</returns>
        public ExamRoom GetById(Guid id)
        {
            var entityData = _dbContext.ExamRoom.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examrooms based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examrooms</returns>/// <exception cref="Exception"></exception>
        public List<ExamRoom> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExamRoom(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examroom</summary>
        /// <param name="model">The examroom data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ExamRoom model)
        {
            model.Id = CreateExamRoom(model);
            return model.Id;
        }

        /// <summary>Updates a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <param name="updatedEntity">The examroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ExamRoom updatedEntity)
        {
            UpdateExamRoom(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <param name="updatedEntity">The examroom data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ExamRoom> updatedEntity)
        {
            PatchExamRoom(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examroom by its primary key</summary>
        /// <param name="id">The primary key of the examroom</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExamRoom(id);
            return true;
        }
        #region
        private List<ExamRoom> GetExamRoom(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ExamRoom.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ExamRoom>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ExamRoom), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ExamRoom, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExamRoom(ExamRoom model)
        {
            _dbContext.ExamRoom.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExamRoom(Guid id, ExamRoom updatedEntity)
        {
            _dbContext.ExamRoom.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExamRoom(Guid id)
        {
            var entityData = _dbContext.ExamRoom.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ExamRoom.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExamRoom(Guid id, JsonPatchDocument<ExamRoom> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ExamRoom.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ExamRoom.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}