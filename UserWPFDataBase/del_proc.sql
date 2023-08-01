use Users
go

create or alter proc delete_positions
@positionId int
as 
delete from Users where PositionId = @positionId
go