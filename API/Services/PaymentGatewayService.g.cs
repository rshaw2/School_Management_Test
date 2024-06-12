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
    /// The paymentgatewayService responsible for managing paymentgateway related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting paymentgateway information.
    /// </remarks>
    public interface IPaymentGatewayService
    {
        /// <summary>Retrieves a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <returns>The paymentgateway data</returns>
        PaymentGateway GetById(Guid id);

        /// <summary>Retrieves a list of paymentgateways based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of paymentgateways</returns>
        List<PaymentGateway> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new paymentgateway</summary>
        /// <param name="model">The paymentgateway data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(PaymentGateway model);

        /// <summary>Updates a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <param name="updatedEntity">The paymentgateway data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, PaymentGateway updatedEntity);

        /// <summary>Updates a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <param name="updatedEntity">The paymentgateway data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<PaymentGateway> updatedEntity);

        /// <summary>Deletes a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The paymentgatewayService responsible for managing paymentgateway related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting paymentgateway information.
    /// </remarks>
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private SchoolManagementTestContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the PaymentGateway class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public PaymentGatewayService(SchoolManagementTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <returns>The paymentgateway data</returns>
        public PaymentGateway GetById(Guid id)
        {
            var entityData = _dbContext.PaymentGateway.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of paymentgateways based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of paymentgateways</returns>/// <exception cref="Exception"></exception>
        public List<PaymentGateway> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetPaymentGateway(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new paymentgateway</summary>
        /// <param name="model">The paymentgateway data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(PaymentGateway model)
        {
            model.Id = CreatePaymentGateway(model);
            return model.Id;
        }

        /// <summary>Updates a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <param name="updatedEntity">The paymentgateway data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, PaymentGateway updatedEntity)
        {
            UpdatePaymentGateway(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <param name="updatedEntity">The paymentgateway data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<PaymentGateway> updatedEntity)
        {
            PatchPaymentGateway(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific paymentgateway by its primary key</summary>
        /// <param name="id">The primary key of the paymentgateway</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeletePaymentGateway(id);
            return true;
        }
        #region
        private List<PaymentGateway> GetPaymentGateway(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PaymentGateway.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PaymentGateway>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PaymentGateway), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PaymentGateway, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreatePaymentGateway(PaymentGateway model)
        {
            _dbContext.PaymentGateway.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdatePaymentGateway(Guid id, PaymentGateway updatedEntity)
        {
            _dbContext.PaymentGateway.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeletePaymentGateway(Guid id)
        {
            var entityData = _dbContext.PaymentGateway.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PaymentGateway.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchPaymentGateway(Guid id, JsonPatchDocument<PaymentGateway> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PaymentGateway.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PaymentGateway.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}