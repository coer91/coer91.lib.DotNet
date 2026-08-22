using coer91.NET;

namespace API.AutoMappers
{
    public static class UserMapper
    {
       
        public static UserDTO ToDTO(this AutoMapper _, User entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

                
        public static User ToEntity(this AutoMapper _, UserDTO dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name
        };


        #region Mapping Collections
        public static List<UserDTO> ToDTO(this AutoMapper _mapper, IEnumerable<User> entities) => [.. entities.Select(_mapper.ToDTO)];
        public static List<User> ToEntity(this AutoMapper _mapper, IEnumerable<UserDTO> dtos) => [.. dtos.Select(_mapper.ToEntity)];
        #endregion
    }
}