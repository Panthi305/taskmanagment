using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using BCrypt.Net;

/*
 * ============================================================================
 * AUTHENTICATION CONTROLLER - SESSION-BASED AUTHENTICATION
 * ============================================================================
 * 
 * This controller handles user authentication using session-based approach.
 * 
 * AUTHENTICATION vs AUTHORIZATION:
 * -------------------------------
 * Authentication: Verifying WHO the user is (login with credentials)
 * Authorization: Verifying WHAT the user can do (role-based permissions)
 * 
 * SESSION-BASED AUTHENTICATION FLOW:
 * ---------------------------------
 * 1. User submits email and password
 * 2. Server validates credentials against database
 * 3. If valid, server creates session and stores user info
 * 4. Session ID is sent to client as HTTP-only cookie
 * 5. Client includes cookie in all subsequent requests
 * 6. Server reads session data from cookie to identify user
 * 7. On logout, session is cleared
 * 
 * SECURITY FEATURES:
 * -----------------
 * - Passwords are hashed using BCrypt (never stored in plain text)
 * - Session cookies are HTTP-only (prevents JavaScript access)
 * - Session expires after 30 minutes of inactivity
 * - Failed login returns generic error (doesn't reveal if email exists)
 * 
 * LINQ USAGE:
 * ----------
 * Where(u => u.Email == loginDto.Email)
 * 
 * Filters users by email address to find matching account
 */

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")] // Maps to /api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * ========================================================================
         * POST /api/auth/login - AUTHENTICATE USER
         * ========================================================================
         * 
         * Validates user credentials and creates a session.
         * 
         * PROCESS:
         * -------
         * 1. Receive LoginDto with email and password
         * 2. Query database for user with matching email (LINQ)
         * 3. Verify password using BCrypt
         * 4. If valid, create session with user data
         * 5. Return user information (without password)
         * 
         * PASSWORD SECURITY:
         * -----------------
         * - Passwords are hashed using BCrypt before storing in database
         * - BCrypt.Verify() compares plain text password with hashed version
         * - Even if database is compromised, passwords cannot be recovered
         * - Each password has unique salt (prevents rainbow table attacks)
         * 
         * SESSION STORAGE:
         * ---------------
         * HttpContext.Session stores:
         * - UserId: For identifying user in subsequent requests
         * - UserRole: For role-based authorization
         * - UserName: For display purposes
         * 
         * SECURITY NOTE:
         * -------------
         * Returns generic "Invalid credentials" message whether email
         * doesn't exist or password is wrong. This prevents attackers
         * from discovering valid email addresses.
         */
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // LINQ query to find user by email
                // Where() filters, FirstOrDefaultAsync() returns first match or null
                var user = await _context.Users
                    .Where(u => u.Email == loginDto.Email)
                    .FirstOrDefaultAsync();

                // Verify user exists and password is correct
                // BCrypt.Verify() compares plain password with hashed password
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Create session - store user information
                // Session data is stored server-side, only session ID sent to client
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail", user.Email);

                // Return user data (excluding sensitive information like password)
                return Ok(new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * POST /api/auth/logout - END USER SESSION
         * ========================================================================
         * 
         * Clears session data and logs out the user.
         * 
         * PROCESS:
         * -------
         * 1. Clear all session data
         * 2. Session cookie becomes invalid
         * 3. User must login again to access protected resources
         */
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // Clear all session data
                HttpContext.Session.Clear();
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        /*
         * ========================================================================
         * GET /api/auth/session - CHECK CURRENT SESSION
         * ========================================================================
         * 
         * Returns current user information if session is valid.
         * Used by Angular frontend to:
         * - Check if user is still logged in
         * - Restore user state after page refresh
         * - Verify session hasn't expired
         * 
         * If session is invalid or expired, returns 200 with authenticated=false.
         */
        [HttpGet("session")]
        public IActionResult GetSession()
        {
            try
            {
                // Try to get user ID from session
                var userId = HttpContext.Session.GetInt32("UserId");
                
                if (userId == null)
                {
                    // Session doesn't exist or has expired
                    // Return 200 so the client doesn't log a failed resource load on first visit.
                    return Ok(new { authenticated = false });
                }

                // Session is valid, return user information
                return Ok(new
                {
                    authenticated = true,
                    id = userId,
                    name = HttpContext.Session.GetString("UserName"),
                    email = HttpContext.Session.GetString("UserEmail"),
                    role = HttpContext.Session.GetString("UserRole")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking session", error = ex.Message });
            }
        }
    }
}
