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
    /// The examinerService responsible for managing examiner related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examiner information.
    /// </remarks>
    public interface IExaminerService
    {
        /// <summary>Retrieves a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <returns>The examiner data</returns>
        Examiner GetById(Guid id);

        /// <summary>Retrieves a list of examiners based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examiners</returns>
        List<Examiner> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new examiner</summary>
        /// <param name="model">The examiner data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Examiner model);

        /// <summary>Updates a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <param name="updatedEntity">The examiner data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Examiner updatedEntity);

        /// <summary>Updates a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <param name="updatedEntity">The examiner data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Examiner> updatedEntity);

        /// <summary>Deletes a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The examinerService responsible for managing examiner related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting examiner information.
    /// </remarks>
    public class ExaminerService : IExaminerService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Examiner class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ExaminerService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <returns>The examiner data</returns>
        public Examiner GetById(Guid id)
        {
            var entityData = _dbContext.Examiner.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of examiners based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of examiners</returns>/// <exception cref="Exception"></exception>
        public List<Examiner> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetExaminer(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new examiner</summary>
        /// <param name="model">The examiner data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Examiner model)
        {
            model.Id = CreateExaminer(model);
            return model.Id;
        }

        /// <summary>Updates a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <param name="updatedEntity">The examiner data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Examiner updatedEntity)
        {
            UpdateExaminer(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <param name="updatedEntity">The examiner data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Examiner> updatedEntity)
        {
            PatchExaminer(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific examiner by its primary key</summary>
        /// <param name="id">The primary key of the examiner</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteExaminer(id);
            return true;
        }
        #region
        private List<Examiner> GetExaminer(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Examiner.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Examiner>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Examiner), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Examiner, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateExaminer(Examiner model)
        {
            _dbContext.Examiner.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateExaminer(Guid id, Examiner updatedEntity)
        {
            _dbContext.Examiner.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteExaminer(Guid id)
        {
            var entityData = _dbContext.Examiner.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Examiner.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchExaminer(Guid id, JsonPatchDocument<Examiner> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Examiner.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Examiner.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}