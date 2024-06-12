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
    /// The paymentService responsible for managing payment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting payment information.
    /// </remarks>
    public interface IPaymentService
    {
        /// <summary>Retrieves a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <returns>The payment data</returns>
        Payment GetById(Guid id);

        /// <summary>Retrieves a list of payments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of payments</returns>
        List<Payment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new payment</summary>
        /// <param name="model">The payment data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Payment model);

        /// <summary>Updates a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <param name="updatedEntity">The payment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Payment updatedEntity);

        /// <summary>Updates a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <param name="updatedEntity">The payment data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Payment> updatedEntity);

        /// <summary>Deletes a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The paymentService responsible for managing payment related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting payment information.
    /// </remarks>
    public class PaymentService : IPaymentService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Payment class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PaymentService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <returns>The payment data</returns>
        public Payment GetById(Guid id)
        {
            var entityData = _dbContext.Payment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of payments based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of payments</returns>/// <exception cref="Exception"></exception>
        public List<Payment> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPayment(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new payment</summary>
        /// <param name="model">The payment data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Payment model)
        {
            model.Id = CreatePayment(model);
            return model.Id;
        }

        /// <summary>Updates a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <param name="updatedEntity">The payment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Payment updatedEntity)
        {
            UpdatePayment(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <param name="updatedEntity">The payment data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Payment> updatedEntity)
        {
            PatchPayment(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific payment by its primary key</summary>
        /// <param name="id">The primary key of the payment</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePayment(id);
            return true;
        }
        #region
        private List<Payment> GetPayment(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Payment.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Payment>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Payment), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Payment, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePayment(Payment model)
        {
            _dbContext.Payment.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePayment(Guid id, Payment updatedEntity)
        {
            _dbContext.Payment.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePayment(Guid id)
        {
            var entityData = _dbContext.Payment.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Payment.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPayment(Guid id, JsonPatchDocument<Payment> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Payment.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Payment.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}