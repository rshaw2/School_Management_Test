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
    /// The questionService responsible for managing question related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting question information.
    /// </remarks>
    public interface IQuestionService
    {
        /// <summary>Retrieves a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <returns>The question data</returns>
        Question GetById(Guid id);

        /// <summary>Retrieves a list of questions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of questions</returns>
        List<Question> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new question</summary>
        /// <param name="model">The question data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Question model);

        /// <summary>Updates a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <param name="updatedEntity">The question data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Question updatedEntity);

        /// <summary>Updates a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <param name="updatedEntity">The question data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Question> updatedEntity);

        /// <summary>Deletes a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The questionService responsible for managing question related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting question information.
    /// </remarks>
    public class QuestionService : IQuestionService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Question class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public QuestionService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <returns>The question data</returns>
        public Question GetById(Guid id)
        {
            var entityData = _dbContext.Question.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of questions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of questions</returns>/// <exception cref="Exception"></exception>
        public List<Question> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetQuestion(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new question</summary>
        /// <param name="model">The question data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Question model)
        {
            model.Id = CreateQuestion(model);
            return model.Id;
        }

        /// <summary>Updates a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <param name="updatedEntity">The question data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Question updatedEntity)
        {
            UpdateQuestion(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <param name="updatedEntity">The question data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Question> updatedEntity)
        {
            PatchQuestion(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific question by its primary key</summary>
        /// <param name="id">The primary key of the question</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteQuestion(id);
            return true;
        }
        #region
        private List<Question> GetQuestion(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Question.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Question>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Question), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Question, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateQuestion(Question model)
        {
            _dbContext.Question.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateQuestion(Guid id, Question updatedEntity)
        {
            _dbContext.Question.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteQuestion(Guid id)
        {
            var entityData = _dbContext.Question.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Question.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchQuestion(Guid id, JsonPatchDocument<Question> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Question.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Question.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}