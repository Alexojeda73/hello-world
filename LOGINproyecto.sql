create proc validar
@usuario varchar (45),
@contraseņa varchar(45),
@tipo varchar (45)
as 
select*from usuario
where nickname=@usuario and contraseņa=@contraseņa and tipoUsuario=@tipo
go