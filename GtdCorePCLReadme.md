As of September 26, 2013 please note the following about the **"Gtd Core PCLs"** solution folder:

- there is a Visual Studio solution folder named "Gtd Core PCLs" in this repository
- "**PCL**" means ".NET Portable Class Library project"
- inside of it there are two projects named "Gtd.**Pcl**.CoreDomain" and "Gtd.**Pcl**.PublishedLanguage"
- these are just copies of the similarly named projects inside of the root of the Visual Studio solution
- they have had code commented out and somewhat randomly modified just to get them to build under PCL profile 78
- the original intent was to reuse these DLLs inside of the "Gtd Mobile" core projects that are also built as PCLs
- we discussed that for now we did NOT reuse these in the PCL mobile project at all and that they are left here only as a reminder to possibly attempt a proper migration of these to PCL in the future to facilitate reuse in PCL projects
- for now they are here for reference only and are not used in any of the executing code inside of this solution