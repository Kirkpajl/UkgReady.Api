![UKG Logo](https://raw.githubusercontent.com/Kirkpajl/UkgReady.Api/master/ukg-logo.png "UKG Logo")

# UKG Ready API implementation in C#.NET

#### IMPORTANT NOTE:
* I am not actively maintaining this repo project.
* If you find issues - please reach out with a PR that would solve it. I would try to push NuGET with updates to help as soon as I can.
* If anyone is interested to take over as maintainer - please contact me, thanks!

---

## Latest NuGet Release:
This library can be used from NuGet channel:

* [UKG Ready API Package](https://www.nuget.org/packages/ukgready.api/) - Version 1.0.0: `Install-Package UkgReady.Api`

**Please note:** This source and nuget are a work-in-progress.  Not all API methods/calls have been included/tested.

## Example Usage

### Get Employee Information

```C#
// Initialize the UkgReady REST Api client
var client = new UkgReadyApiClient();

// Authenticate with the UKG Ready service
await client.LoginAsync("[API KEY]", "[USERNAME]", "[PASSWORD]", "[COMPANYNAME]");

// Retrieve the basic employee info
var employees = await ukgReady.GetEmployeesAsync();
employees = employees
	.OrderBy(e => e.LastName)
	.ThenBy(e => e.FirstName)
	.ToArray();

// Write the employee info to the console window
foreach (var employee in employees)
{
	Console.WriteLine($"  [{employee.Id}]:  {employee.LastName}, {employee.FirstName} - Emp Id:  {employee.EmployeeId}");
}
```

## Documentation:
For further details on how to use/integrate the UkgReady.Api package, please refer to the repository wiki page.

[UKG Ready API .NET SDK WIKI](https://github.com/kirkpajl/ukgready.api/wiki)

## Issues / bugs:
If you have a query, issues or bugs, it means that you have shown interest in this project, and I thank you for that.
Feel free to ask, suggest, report issue or post a bug [here](https://github.com/kirkpajl/ukgready.api/issues) in context of this library use.

**Please note:** If your query/issue/bug is related to the UKG Ready API, I recommend posting it to the official [UKG Support](https://www.ukg.com/support/) forum.

You can find all of the methods to connect with me at my [blog](https://joshuakirkpatrick.com/contact)

## References:

* [UKG Ready API](https://secure.workforceready.com.au/ta/docs/rest/public/) - Official documentation
* [My Blog](https://joshuakirkpatrick.com/) - My personal blog

## Credits / Disclaimer:

* UKG logo used in this readme file is owned by and copyright of UKG.
* I am not affiliated with UKG, this work is solely undertaken by me.
* This library is not or part of the Official set of libraries from UKG and hence can be referred as Third party library for UKG using .NET.

## License

This work is [licensed](https://github.com/kirkpajl/ukgready.api/blob/master/LICENSE) under:

The MIT License (MIT)
Copyright (c) 2023 Josh Kirkpatrick