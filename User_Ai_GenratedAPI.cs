/// <summary>
/// AI Copilot Generated Method
/// Purpose: Handles creation of a new user using Entity Framework Core.
/// Includes password generation, hashing, audit tracking, and email notification.
/// </summary>
public async Task<AppResponse<UserDetailsDto>> AddUserAsync_AI(UserDetailsDto dto, CancellationToken cancellationToken)
{
    try
    {
        // Step 1: Generate secure password for first-time user creation
        var generatedPassword = _passwordSecurityService.GeneratePassword();

        // Step 2: Hash the password before persisting into database
        var passwordHash = _passwordSecurityService.HashPassword(generatedPassword);

        // Step 3: Map DTO to Entity model
        var userEntity = new User
        {
            UserGuidId = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Email = dto.Email,
            ContactNumber = dto.ContactNumber,
            RoleGuidId = dto.RoleGuidId,
            DepartmentGuidId = dto.DepartmentGuidId,
            PasswordHash = passwordHash,

            // Audit fields
            CreatedBy = _currentUserService.UserGuidId,
            CreatedIp = _currentUserService.IpAddress,
            CreatedOn = DateTime.UtcNow
        };

        // Step 4: Add entity to DbContext
        await _dbContext.Users.AddAsync(userEntity, cancellationToken);

        // Step 5: Persist changes to database
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Step 6: Send onboarding email notification
        await SendRegistrationEmailAsync(userEntity, generatedPassword, dto.FrontendUrl, cancellationToken);

        // Step 7: Return standardized success response
        return AppResponse.Success(
            _mapper.Map<UserDetailsDto>(userEntity),
            "User created successfully via AI-generated method.",
            HttpStatusCodes.OK);
    }
    catch (Exception ex)
    {
        // Centralized error handling
        return AppResponse.Fail<UserDetailsDto>(dto, ex.Message, HttpStatusCodes.BadRequest);
    }
}
