# aráchni
API based web crawler

The API will run leveraging the kestrel web server.

## Run 
There are 2 ways to start the webserver to run the api:
1. Execute the pre-release version
2. Run it with Visual Studio 2019+

### Release version

[Download the pre-release](https://github.com/overbit/arachni-api/releases/tag/v0.9.0) and follow the instructions for the right platform. 
The release version is a selfcontained executable it should not need any additional framework to be installed.

#### Windows 10+ x64

Simply run the arachni.exe and wait until the console app will start listening for traffic.

#### MacOS 10.14+ x64

Make arachni executable by running `sudo chmod +x arachni` and start as a normal bash script `./arachni`.

## API specification
This api is documented through the OpenAPI v3 specification and it is accessible at 
`http://localhost:5000/swagger` 
`https://localhost:5001/swagger`

## Compile

Again two options:
1. dotnet CLI by running `dotnet run arachni.sln`  (.NET Core 3.1 SDK required)
2. In Visual Studio 2019+ 
3. Docker 