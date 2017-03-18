use [CT.DB]

--create table Processes
--(
--ProcessId int primary key,
--ProcessType varchar(30) not null
--)

--create table [Checkpoints]
--(
--CheckpointId int primary key,
--CheckpointType varchar(40) not null,
--Serial int not null,
--Duration int not null,
--ProcessId int references Processes(ProcessId)
--)

drop table Flights

create table Flights
(
FlightId int primary key,
FlightSerial int not null,
IsAlive bit not null,
CheckpointId int null references Checkpoints(CheckpointId)
)

--insert into dbo.Processes (ProcessType) values('LandingProcess')
--insert into dbo.Processes (ProcessType) values('DepartingProcess')

--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(1, 'Landing', 2, 1028)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(2, 'Landing', 2, 1028)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(3, 'Landing', 2, 1028)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(4, 'RunwayLanded', 3, 1028)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(5, 'StandbyLanded', 2, 1028)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(6, 'ParkingUnloading', 5, 1028)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(7, 'ParkingBoarding', 5, 1029)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(8, 'StandbyDeparting', 2, 1029)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(4, 'RunwayDeparting', 3, 1029)
--insert into dbo.Checkpoints (Serial, CheckpointType, Duration, ProcessId) values(9, 'Departed', 2, 1029)

alter table Flights
add FlightSerial int not null

-----------------------------

update Flights set CheckpointId = null

select * 
from dbo.Checkpoints

select *
from dbo.Processes

select *
from dbo.Flights

select f.FlightId, f.IsAlive, p.ProcessType, cp.CheckpointType, cp.Serial[CheckpointSerial], cp.Duration 
from Checkpoints cp join Flights f on cp.CheckpointId = f.CheckpointId
					join Processes p on p.ProcessId = cp.ProcessId

select cp.CheckpointType, cp.Serial, cp.Duration, p.ProcessType
from dbo.Checkpoints cp join dbo.Processes p on cp.ProcessId = p.ProcessId

delete from dbo.Checkpoints
where CheckpointId > 1124

delete from dbo.Processes
where ProcessId > 0

delete from dbo.Flights
where FlightId > 0

--sp_configure 'show advanced options', 1
--RECONFIGURE
--GO
--sp_configure 'awe enabled', 1
--RECONFIGURE
--GO
--sp_configure 'max server memory', 6144
--RECONFIGURE
--GO