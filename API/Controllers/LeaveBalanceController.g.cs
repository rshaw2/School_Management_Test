using Microsoft.AspNetCore.Mvc;
using SchoolManagementTest.Models;
using SchoolManagementTest.Services;
using SchoolManagementTest.Entities;
using SchoolManagementTest.Filter;
using SchoolManagementTest.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace SchoolManagementTest.Controllers
{
    /// <summary>
    /// Controller responsible for managing leavebalance related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting leavebalance information.
    /// </remarks>
    [Route("api/leavebalance")]
    [Authorize]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly ILeaveBalanceService _leaveBalanceService;

        /// <summary>
        /// Initializes a new instance of the LeaveBalanceController class with the specified context.
        /// </summary>
        /// <param name="ileavebalanceservice">The ileavebalanceservice to be used by the controller.</param>
        public LeaveBalanceController(ILeaveBalanceService ileavebalanceservice)
        {
            _leaveBalanceService = ileavebalanceservice;
        }

        /// <summary>Adds a new leavebalance</summary>
        /// <param name="model">The leavebalance data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("LeaveBalance",Entitlements.Create)]
        public IActionResult Post([FromBody] LeaveBalance model)
        {
            var id = _leaveBalanceService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of leavebalances based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of leavebalances</returns>
        [HttpGet]
        [UserAuthorize("LeaveBalance",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _leaveBalanceService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <returns>The leavebalance data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("LeaveBalance",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _leaveBalanceService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("LeaveBalance",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _leaveBalanceService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <param name="updatedEntity">The leavebalance data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("LeaveBalance",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] LeaveBalance updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _leaveBalanceService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific leavebalance by its primary key</summary>
        /// <param name="id">The primary key of the leavebalance</param>
        /// <param name="updatedEntity">The leavebalance data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("LeaveBalance",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<LeaveBalance> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _leaveBalanceService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}