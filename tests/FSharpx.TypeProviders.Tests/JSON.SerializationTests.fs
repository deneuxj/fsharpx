﻿module FSharp.TypeProviders.Tests.JSONWriterTests

open NUnit.Framework
open FSharpx.TypeProviders.JSON
open FsUnit

[<Test>] 
let ``Can serialize empty document``() = 
    emptyJObject.ToString()
    |> should equal "{}"

[<Test>] 
let ``Can serialize document with single property``() =
    (emptyJObject |> addProperty "firstName" (Text "John")).ToString()
    |> should equal "{\"firstName\":\"John\"}"


[<Test>] 
let ``Can serialize document with booleans``() =
    (emptyJObject |> addProperty "aa" (Boolean true) |> addProperty "bb" (Boolean false)).ToString()
    |> should equal "{\"aa\":true,\"bb\":false}"

[<Test>]
let ``Can serialize document with array, null and number``() =
    let text = "{\"items\":[{\"id\":\"Open\"},null,{\"id\":25}]}"
    let json = parse text
    json.ToString() |> should equal text

open System.Xml.Linq

[<Test>]
let ``Can serialize document to XML``() =
    let text = "{\"items\": [{\"id\": \"Open\"}, null, {\"id\": 25}]}"
    let json = parse text
    let xml = json.ToXml() |> Seq.head 
    let expectedXml = XElement.Parse("<items><item><id>Open</id></item><item /><item><id>25</id></item></items>")
    xml.ToString() |> should equal (expectedXml.ToString())