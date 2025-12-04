using AutoMapper;
using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laboratory_Service.Application.UnitTest.Comments
{
    public class AddCommentCommandHandlerTests
    {
        private readonly Mock<ICommentRepository> _commentRepoMock;
        private readonly IMapper _mapper;

        public AddCommentCommandHandlerTests()
        {
            _commentRepoMock = new Mock<ICommentRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                // Ánh xạ vẫn sử dụng tên đầy đủ hoặc tên đơn giản nếu namespace Domain.Entity được 'using'
                cfg.CreateMap<CreateCommentDto, Domain.Entity.Comment>()
                    .ForMember(dest => dest.TestResultId, opt => opt.MapFrom(src => 
                        src.TestResultId != null && src.TestResultId.Count > 0 
                            ? (int?)src.TestResultId[0] 
                            : (int?)null))
                    .ForMember(dest => dest.CommentId, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.DeletedDate, opt => opt.Ignore());
            });
            config.AssertConfigurationIsValid();

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldCreateComment_WhenValidRequest()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                TestOrderId = Guid.Parse("8e200a3c-6fe0-46b8-a628-98fa3a5eed3b"),
                TestResultId = new List<int> { 10 },
                Message = "New test comment"
            };

            string jwtUserId = "user99";

            var command = new AddCommentCommand(dto, jwtUserId);

            // Setup: Mock entity to be returned after AddAsync sets CommentId
            var createdEntity = new Domain.Entity.Comment
            {
                CommentId = 123,
                TestOrderId = dto.TestOrderId,
                TestResultId = dto.TestResultId.FirstOrDefault(),
                Message = dto.Message,
                CreatedBy = jwtUserId
            };

            Domain.Entity.Comment? capturedEntity = null;
            
            _commentRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Domain.Entity.Comment>()))
                .Callback<Domain.Entity.Comment>(entity => 
                {
                    entity.CommentId = 123;
                    capturedEntity = entity;
                })
                .Returns(Task.CompletedTask);

            _commentRepoMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var handler = new AddCommentCommandHandler(_commentRepoMock.Object, _mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result > 0);
            Assert.Equal(123, result);
            Assert.NotNull(capturedEntity);
            Assert.Equal(jwtUserId, capturedEntity.CreatedBy);
            Assert.Equal(dto.Message, capturedEntity.Message);
            Assert.Equal(dto.TestOrderId, capturedEntity.TestOrderId);
            var expectedTestResultId = dto.TestResultId != null && dto.TestResultId.Count > 0 ? (int?)dto.TestResultId[0] : null;
            Assert.Equal(expectedTestResultId, capturedEntity.TestResultId);

            _commentRepoMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entity.Comment>()), Times.Once);
            _commentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}