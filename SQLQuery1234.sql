use controlAlumnos

create table Alumnos(
idAlumno int  primary key identity (1,1),
idCurso int foreign key (idCurso) references Curso (idCurso),
idApoderado int foreign key (idApoderado) references Apoderado (idApoderado),
rut int,
nombre varchar (30)not null,
apellido nvarchar (30),
fechaNacimiento date

)


create table Curso(
idCurso int primary key identity (1,1),
idHorario int foreign key (idHorario) references Horario (idHorario),
nombre nvarchar (20)
)





create table Apoderado (
idApoderado int primary key identity (1,1),
nombre nvarchar (30),
direccion nvarchar (40),
telefono int,
email nvarchar (50)
)



create table RegistroAtrasos(
idRegistro int primary key identity (1,1),
fechaRegistro date,
horaIngreso time,
idAlumno int foreign key (idAlumno) references Alumnos (idAlumno),
idPersonal int foreign key (idPersonal) references Usuario (idPersonal),
motivo nvarchar (150),
observacion nvarchar (50),
autorizacion nvarchar (100)

)


create table Horario(
idHorario int primary key identity(1,1),
inicio time,
termino time,
nombre nvarchar (30),
jornada nvarchar (25)

)

create table Usuario(
idPersonal int primary key identity (1,1),
nombre nvarchar (25),
usuario nvarchar (50),
clave nvarchar (25)
)

create table Huella(
idEnrolar int primary key identity (1,1),
idAlumno int foreign key (idAlumno) references Alumnos (idAlumno),
Huella numeric
)