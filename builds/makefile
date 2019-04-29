NUGET_ADDR = http://192.168.8.11:16969/
NUGET_KEY = joycoding!@12

PJ = 
PROJECT_DIRS = $(foreach proj,${PJ},../src/Droplet.${proj}) 
PROJECT_CSPROJ = $(foreach proj,${PJ},../src/Droplet.${proj}/Droplet.${proj}.csproj)

ifneq (${PJA}, )
	PJ = AutoDI AutoMapper Bootstrapper Data Data.EntityFrameworkCore Discovery Discovery.Consul EventBus EventBus.RabbitMQ GrpcHost Module RawRabbit ServiceProxy.Grpc Utility
endif

clean:
	$(foreach proj,${PROJECT_DIRS}, rm -rf ${proj}/bin ; )
	$(foreach proj,${PROJECT_DIRS}, rm -rf ${proj}/obj ; )

build:clean
	$(foreach proj,${PROJECT_CSPROJ},dotnet build ${proj} -c Release ; )

push:build
	$(foreach proj,${PROJECT_DIRS},./nuget.exe push ${proj}/bin/Release/*.nupkg ${NUGET_KEY} -Source ${NUGET_ADDR} ; )

test:
	echo ${PJ}
	echo ${PROJECT_DIRS}

.PHONY: clean build push test
