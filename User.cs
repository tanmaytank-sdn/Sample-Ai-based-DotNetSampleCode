
/* CODE By the Normal dev */
public async Task<AppResponse<UserDetailsDto>> AddUserAsync(UserDetailsDto dto, CancellationToken cancellationToken)
{
    try
    {
        var generatedPassword = _passwordSecurityService.GeneratePassword();
        var passwordHash = _passwordSecurityService.HashPassword(generatedPassword);

        var user = new User
        {
            UserGuidId = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Username,
            Designation = dto.Designation,
            EmploymentId = dto.EmploymentId,
            DateOfJoining = dto.DateOfJoining,
            ContactNumber = dto.ContactNumber,
            Email = dto.Email,
            RoleGuidId = dto.RoleGuidId,
            DepartmentGuidId = dto.DepartmentGuidId,
            Status = dto.Status,
            UniquePin = dto.UniquePin,
            PasswordHash = passwordHash,
            ProfileImage = dto.ProfileImage,
            CreatedBy = _currentUserService.UserGuidId,
            CreatedIp = _currentUserService.IpAddress,
            CreatedOn = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await SendRegistrationEmailAsync(user, generatedPassword, dto.FrontendUrl, cancellationToken);

        return AppResponse.Success(
            _mapper.Map<UserDetailsDto>(user),
            $"{EntityNames.User} {CommonMessageTemplates.AddedSuccessfully}",
            HttpStatusCodes.OK);
    }
    catch (Exception ex)
    {
        return AppResponse.Fail<UserDetailsDto>(dto, ex.Message, HttpStatusCodes.BadRequest);
    }
}

public async Task<AppResponse<UserDetailsDto>> UpdateUserAsync(UserDetailsDto dto, CancellationToken cancellationToken)
{
    try
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.UserGuidId == dto.UserGuidId, cancellationToken);

        if (user == null)
        {
            return AppResponse.Fail<UserDetailsDto>(
                null,
                CommonMessageTemplates.NotFound,
                HttpStatusCodes.NotFound);
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Username = dto.Username;
        user.Designation = dto.Designation;
        user.EmploymentId = dto.EmploymentId;
        user.DateOfJoining = dto.DateOfJoining;
        user.ContactNumber = dto.ContactNumber;
        user.Email = dto.Email;
        user.RoleGuidId = dto.RoleGuidId;
        user.DepartmentGuidId = dto.DepartmentGuidId;
        user.Status = dto.Status;
        user.UniquePin = dto.UniquePin;
        user.ProfileImage = dto.ProfileImage;
        user.UpdatedBy = _currentUserService.UserGuidId;
        user.UpdatedIp = _currentUserService.IpAddress;
        user.UpdatedOn = DateTime.UtcNow;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return AppResponse.Success(
            _mapper.Map<UserDetailsDto>(user),
            $"{EntityNames.User} {CommonMessageTemplates.UpdatedSuccessfully}",
            HttpStatusCodes.OK);
    }
    catch (Exception ex)
    {
        return AppResponse.Fail<UserDetailsDto>(dto, ex.Message, HttpStatusCodes.BadRequest);
    }
}

private async Task SendRegistrationEmailAsync(User user, string password, string frontendUrl, CancellationToken cancellationToken)
{
    var loginUrl = $"{frontendUrl}/auth/login";

    var templatePath = Path.Combine(
        _env.WebRootPath,
        "EmailTemplates",
        "registration.html");

    if (!File.Exists(templatePath))
        throw new FileNotFoundException($"Email template not found: {templatePath}");

    var emailBody = File.ReadAllText(templatePath)
        .Replace("{fullName}", $"{user.FirstName} {user.LastName}")
        .Replace("{email}", user.Email)
        .Replace("{username}", user.Username)
        .Replace("{password}", password)
        .Replace("{loginUrl}", loginUrl)
        .Replace("{CurrentYear}", DateTime.Now.Year.ToString());

    await _emailService.SendEmailAsync(
        new EmailOptions
        {
            ToEmail = user.Email,
            Subject = "Welcome to CBWMS - Login Credentials",
            Body = emailBody,
            IsHtml = true
        },
        cancellationToken);
}

//========================================================================================================================================

/* CODE Genrated by the AI */
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
