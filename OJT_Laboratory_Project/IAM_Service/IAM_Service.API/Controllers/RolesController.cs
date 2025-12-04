using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Roles.Command;
using IAM_Service.Application.Roles.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace IAM_Service.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get paged list of roles with optional search and sorting.
        /// Query params:
        /// - search: keyword across role and privilege names
        /// - page, pageSize: paging
        /// - sortBy: id|name|code|description; sortDesc: true|false
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            OperationId = "GetRoles",
            Summary = "Get list of roles",
            Description = "Get list by query params",
            Tags = new[] { "Roles" }
        )]
        [Authorize(Policy = "CanViewRole")]
        public async Task<IActionResult> Get([FromQuery] GetRolesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a role by its ID.
        /// </summary>
        [HttpGet("{id:int}", Name = nameof(GetById))]
        [SwaggerOperation(
            OperationId = "GetRoleById",
            Summary = "Get a role by ID",
            Description = "Retrieves a single role by its unique identifier.",
            Tags = new[] { "Roles" }
        )]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "CanViewRole")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetRoleByIdQuery { RoleId = id };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Create a new role (CQRS Command).
        /// - AC01: New role must not exist (unique code)
        /// - AC02: If privileges not provided → assign default 'READ_ONLY'
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            OperationId = "CreateRole",
            Summary = "Create a new role",
            Description = "AC01: Role not exists. AC02: Default privilege READ_ONLY if none provided.",
            Tags = new[] { "Roles" }
        )]
        [Authorize(Policy = "CanCreateRole")]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
        {
            try
            {
                var command = new CreateRoleCommand(dto);
                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetById), new { id = result.RoleId }, result);
            }
            catch (InvalidOperationException ex)
            {
                // AC01: Role already exists
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing role (CQRS Command).
        /// - AC01: Role must exist.
        /// - AC02: Updated role code must remain unique.
        /// </summary>
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            OperationId = "UpdateRole",
            Summary = "Update an existing role",
            Description = "AC01: Role must exist. AC02: Updated code must remain unique.",
            Tags = new[] { "Roles" }
        )]
        [Authorize(Policy = "CanUpdateRole")]
        public async Task<IActionResult> Update(int id, [FromBody, Required] UpdateRoleDto dto)
        {
            try
            {
                var command = new UpdateRoleCommand(id, dto);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                // AC01: Role not found
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // AC02: Duplicate code
                return Conflict(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a role (CQRS Command).
        /// - AC01: Role must exist.
        /// - AC02: Cannot delete a role assigned to users.
        /// </summary>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            OperationId = "DeleteRole",
            Summary = "Delete a role",
            Description = "AC01: Role must exist. AC02: Cannot delete a role assigned to users.",
            Tags = new[] { "Roles" }
        )]
        [Authorize(Policy = "CanDeleteRole")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteRoleCommand(id);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    // AC01: Role not found
                    return NotFound(new { message = $"Role with ID {id} not found." });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // AC02: Role assigned to users
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
