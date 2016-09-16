namespace SampleTests

open FsCheck.Xunit
open SampleConsole

module SampleTests = 

    [<Property>]
    let ``Diff should work`` a b =
        diff a b = a - b 
