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

