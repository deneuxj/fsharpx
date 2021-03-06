﻿module FSharpx.Tests.BinaryIterateeTest

open System
open FSharpx
open FSharpx.ByteString
open FSharpx.Iteratee
open FSharpx.Iteratee.Binary
open NUnit.Framework
open FsUnit

[<Test>]
let ``test length should calculate the length of the list without modification``() =
  let actual = enumerate (create [|1uy;2uy;3uy|]) length |> run 
  actual |> should equal 3

[<Test>]
let ``test length should calculate the length of the list without modification all at once``() =
  let actual = enumeratePure1Chunk (create [|1uy;2uy;3uy|]) length |> run 
  actual |> should equal 3

[<Test>]
let ``test length should calculate the length of the list without modification when chunked``() =
  let actual = enumeratePureNChunk 2 (create [|1uy;2uy;3uy|]) length |> run 
  actual |> should equal 3

let testPeekAndHead = [|
  [| box ByteString.empty; box None |]
  [| box (singleton 'c'B); box (Some 'c'B) |]
  [| box (create "char"B); box (Some 'c'B) |]
|]

[<Test>]
[<TestCaseSource("testPeekAndHead")>]
let ``test peek should return the value without removing it from the stream``(input, expected:byte option) =
  let actual = enumerate input peek |> run 
  actual |> should equal expected

[<Test>]
[<TestCaseSource("testPeekAndHead")>]
let ``test peek should return the value without removing it from the stream at once``(input, expected:byte option) =
  let actual = enumeratePure1Chunk input peek |> run 
  actual |> should equal expected

[<Test>]
[<TestCaseSource("testPeekAndHead")>]
let ``test peek should return the value without removing it from the stream when chunked``(input, expected:byte option) =
  let actual = enumeratePureNChunk 2 input peek |> run 
  actual |> should equal expected

[<Test>]
[<TestCaseSource("testPeekAndHead")>]
let ``test head should return the value and remove it from the stream``(input, expected:byte option) =
  let actual = enumerate input head |> run
  actual |> should equal expected

[<Test>]
[<TestCaseSource("testPeekAndHead")>]
let ``test head should return the value and remove it from the stream at once``(input, expected:byte option) =
  let actual = enumeratePure1Chunk input head |> run
  actual |> should equal expected

[<Test>]
[<TestCaseSource("testPeekAndHead")>]
let ``test head should return the value and remove it from the stream when chunked``(input, expected:byte option) =
  let actual = enumeratePureNChunk 2 input head |> run
  actual |> should equal expected

[<Test>]
[<Sequential>]
let ``test drop should drop the first n items``([<Values(0,1,2,3,4,5,6,7,8,9)>] x) =
  let drop2Head = iteratee {
    do! drop x
    return! head }
  let actual = enumerate (create [| 0uy..9uy |]) drop2Head |> run
  actual |> should equal (Some(byte x))

[<Test>]
[<Sequential>]
let ``test drop should drop the first n items at once``([<Values(0,1,2,3,4,5,6,7,8,9)>] x) =
  let drop2Head = iteratee {
    do! drop x
    return! head }
  let actual = enumeratePure1Chunk (create [| 0uy..9uy |]) drop2Head |> run
  actual |> should equal (Some(byte x))

[<Test>]
[<Sequential>]
let ``test drop should drop the first n items when enumerating in chunks``([<Values(0,1,2,3,4,5,6,7,8,9)>] x) =
  let drop2Head = iteratee {
    do! drop x
    return! head }
  let actual = enumeratePureNChunk 5 (create [| 0uy..9uy |]) drop2Head |> run
  actual |> should equal (Some(byte x))

[<Test>]
let ``test dropWhile should drop anything before the first space``() =
  let dropWhile2Head = iteratee {
    do! dropWhile ((<>) ' 'B)
    return! head }
  let actual = enumerate (create "Hello world"B) dropWhile2Head |> run
  actual |> should equal (Some ' 'B)

[<Test>]
let ``test dropWhile should drop anything before the first space at once``() =
  let dropWhile2Head = iteratee {
    do! dropWhile ((<>) ' 'B)
    return! head }
  let actual = enumeratePure1Chunk (create "Hello world"B) dropWhile2Head |> run
  actual |> should equal (Some ' 'B)

[<Test>]
let ``test dropWhile should drop anything before the first space when chunked``() =
  let dropWhile2Head = iteratee {
    do! dropWhile ((<>) ' 'B)
    return! head }
  let actual = enumeratePureNChunk 2 (create "Hello world"B) dropWhile2Head |> run
  actual |> should equal (Some ' 'B)

[<Test>]
let ``test dropUntil should drop anything before the first space``() =
  let dropUntil2Head = iteratee {
    do! dropUntil ((=) ' 'B)
    return! head }
  let actual = enumerate (create "Hello world"B) dropUntil2Head |> run
  actual |> should equal (Some ' 'B)

[<Test>]
let ``test dropUntil should drop anything before the first space at once``() =
  let dropUntil2Head = iteratee {
    do! dropUntil ((=) ' 'B)
    return! head }
  let actual = enumeratePure1Chunk (create "Hello world"B) dropUntil2Head |> run
  actual |> should equal (Some ' 'B)

[<Test>]
let ``test dropUntil should drop anything before the first space when chunked``() =
  let dropUntil2Head = iteratee {
    do! dropUntil ((=) ' 'B)
    return! head }
  let actual = enumeratePureNChunk 2 (create "Hello world"B) dropUntil2Head |> run
  actual |> should equal (Some ' 'B)
  
[<Test>]
[<Sequential>]
let ``test take should take the first n items``([<Values(0,1,2,3,4,5,6,7,8,9,10)>] x) =
  let input = create [|0uy..9uy|]
  let expected = ByteString.take x input
  let actual = enumerate input (take x) |> run
  actual |> should equal expected

[<Test>]
[<Sequential>]
let ``test take should take the first n items at once``([<Values(0,1,2,3,4,5,6,7,8,9,10)>] x) =
  let input = create [|0uy..9uy|]
  let expected = ByteString.take x input
  let actual = enumeratePure1Chunk input (take x) |> run
  actual |> should equal expected

[<Test>]
[<Sequential>]
let ``test take should take the first n items when chunked``([<Values(0,1,2,3,4,5,6,7,8,9,10)>] x) =
  let input = create [|0uy..9uy|]
  let expected = ByteString.take x input
  let actual = enumeratePureNChunk 2 input (take x) |> run
  actual |> should equal expected

[<Test>]
let ``test takeWhile should take anything before the first space``() =
  let input = "Hello world"B
  let expected = BS(input, 0, 5)
  let actual = enumerate (create input) (takeWhile ((<>) ' 'B)) |> run
  actual |> should equal expected

[<Test>]
let ``test takeWhile should take anything before the first space at once``() =
  let input = "Hello world"B
  let expected = BS(input, 0, 5)
  let actual = enumeratePure1Chunk (create input) (takeWhile ((<>) ' 'B)) |> run
  actual |> should equal expected

[<Test>]
let ``test takeWhile should take anything before the first space when enumerating in chunks``() =
  let input = "Hello world"B
  let actual = enumeratePureNChunk 2 (create input) (takeWhile ((<>) ' 'B)) |> run
  actual |> should equal (BS(input, 0, 5))

[<Test>]
let ``test takeUntil should correctly split the input``() =
  let input = "abcde"B
  let actual = enumerate (create input) (takeUntil ((=) 'c'B)) |> run
  actual |> should equal (BS(input, 0, 2))

[<Test>]
let ``test takeUntil should correctly split the input at once``() =
  let input = "abcde"B
  let actual = enumeratePure1Chunk (create input) (takeUntil ((=) 'c'B)) |> run
  actual |> should equal (BS(input, 0, 2))

[<Test>]
let ``test takeUntil should correctly split the input when enumerating in chunks``() =
  let input = "abcde"B
  let actual = enumeratePureNChunk 2 (create input) (takeUntil ((=) 'c'B)) |> run
  actual |> should equal (BS(input, 0, 2))

let takeUntilTests = [|
  [| box ""B; box ByteString.empty; box ByteString.empty |]
  [| box "\r"B; box ByteString.empty; box (singleton '\r'B) |]
  [| box "\n"B; box ByteString.empty; box (singleton '\n'B) |]
  [| box "\r\n"B; box ByteString.empty; box (create "\r\n"B) |]
  [| box "line1"B; box ByteString.empty; box ByteString.empty |]
  [| box "line1\n"B; box (create "line1"B); box (singleton '\n'B) |]
  [| box "line1\r"B; box (create "line1"B); box (singleton '\r'B) |]
  [| box "line1\r\n"B; box (create "line1"B); box (create "\r\n"B) |]
|]

[<Ignore("heads and readLines do not correctly return a correct result when the input is chunked and a \r\n is encountered in different chunks.")>]
[<Test>]
[<TestCaseSource("takeUntilTests")>]
let ``test takeUntilNewline should split strings on a newline character at once``(input, expectedRes:BS, expectedRem:BS) =
  let isNewline c = c = '\r'B || c = '\n'B
  let res, rem =
    match enumerate (create input) (takeUntil isNewline) with
    | Done(res, (Chunk rem)) -> res, rem
    | Continue _ -> ByteString.empty, ByteString.empty
    | _ -> failwith "Unrecognized test result"
  res |> should equal expectedRes
  rem |> should equal expectedRem

[<Test>]
[<TestCaseSource("takeUntilTests")>]
let ``test takeUntilNewline should split strings on a newline character``(input, expectedRes:BS, expectedRem:BS) =
  let isNewline c = c = '\r'B || c = '\n'B
  let res, rem =
    match enumeratePure1Chunk (create input) (takeUntil isNewline) with
    | Done(res, (Chunk rem)) -> res, rem
    | Continue _ -> ByteString.empty, ByteString.empty
    | _ -> failwith "Unrecognized test result"
  res |> should equal expectedRes
  rem |> should equal expectedRem

[<Test>]
let ``test heads should count the number of characters in a set of headers when enumerated one byte at a time``() =
  let actual = enumerate (ByteString.ofString "abd") (heads (ByteString.ofString "abc")) |> run
  actual |> should equal 2

[<Test>]
let ``test heads should count the number of characters in a set of headers``() =
  let actual = enumeratePure1Chunk (ByteString.ofString "abd") (heads (ByteString.ofString "abc")) |> run
  actual |> should equal 2

[<Test>]
let ``test heads should count the number of characters in a set of headers when enumerating in chunks``() =
  let actual = enumeratePureNChunk 2 (ByteString.ofString "abd") (heads (ByteString.ofString "abc")) |> run
  actual |> should equal 2

[<Test>]
let ``test heads should count the correct number of newline characters in a set of headers when enumerated one byte at a time``() =
  let isNewline c = c = '\r'B || c = '\n'B
  let readUntilNewline = takeUntil isNewline >>= fun bs -> heads (create "\r\n"B)
  let actual = enumerate (ByteString.ofString "abc\r\n") readUntilNewline |> run
  actual |> should equal 2

[<Test>]
let ``test heads should count the correct number of newline characters in a set of headers``() =
  let isNewline c = c = '\r'B || c = '\n'B
  let readUntilNewline = takeUntil isNewline >>= fun bs -> heads (create "\r\n"B)
  let actual = enumeratePure1Chunk (ByteString.ofString "abc\r\n") readUntilNewline |> run
  actual |> should equal 2

[<Test>]
let ``test heads should count the correct number of newline characters in a set of headers when chunked``() =
  let isNewline c = c = '\r'B || c = '\n'B
  let readUntilNewline = takeUntil isNewline >>= fun bs -> heads (create "\r\n"B)
  let actual = enumeratePureNChunk 2 (ByteString.ofString "abc\r\n") readUntilNewline |> run
  actual |> should equal 2

let readLinesTests = [|
  [| box ""B; box (Choice1Of2 []:Choice<BS list, BS list>) |]
  [| box "\r"B; box (Choice2Of2 []:Choice<BS list, BS list>) |]
  [| box "\n"B; box (Choice2Of2 []:Choice<BS list, BS list>) |]
  [| box "\r\n"B; box (Choice2Of2 []:Choice<BS list, BS list>) |]
  [| box "line1"B; box (Choice1Of2 []:Choice<BS list, BS list>) |]
  [| box "line1\n"B; box (Choice1Of2 [BS"line1"B]:Choice<BS list, BS list>) |]
  [| box "line1\r"B; box (Choice1Of2 [BS"line1"B]:Choice<BS list, BS list>) |]
  [| box "line1\r\n"B; box (Choice1Of2 [BS"line1"B]:Choice<BS list, BS list>) |]
  [| box "line1\r\nline2"B; box (Choice1Of2 [BS"line1"B]:Choice<BS list, BS list>) |]
  [| box "line1\r\nline2\r\n"B; box (Choice1Of2 [BS"line1"B;BS"line2"B]:Choice<BS list, BS list>) |]
  [| box "line1\r\nline2\r\n\r\n"B; box (Choice2Of2 [BS"line1"B;BS"line2"B]:Choice<BS list, BS list>) |]
  [| box "line1\r\nline2\r\nline3\r\nline4\r\nline5"B; box (Choice1Of2 [BS"line1"B;BS"line2"B;BS"line3"B;BS"line4"B]:Choice<BS list, BS list>) |]
  [| box "line1\r\nline2\r\nline3\r\nline4\r\nline5\r\n"B
     box (Choice1Of2 [BS"line1"B;BS"line2"B;BS"line3"B;BS"line4"B;BS"line5"B]:Choice<BS list, BS list>) |]
  [| box "PUT /file HTTP/1.1\r\nHost: example.com\rUser-Agent: X\nContent-Type: text/plain\r\n\r\n1C\r\nbody line 2\r\n\r\n7"B
     box (Choice2Of2 [BS"PUT /file HTTP/1.1"B;BS"Host: example.com"B;BS"User-Agent: X"B;BS"Content-Type: text/plain"B]:Choice<BS list, BS list>) |]
|]

[<Test>]
[<TestCaseSource("readLinesTests")>]
let ``test readLines should return the lines from the input``(input, expected:Choice<BS list, BS list>) =
  let actual = enumeratePure1Chunk (create input) readLines |> run
  actual |> should equal expected

[<Ignore("heads and readLines do not correctly return a correct result when the input is chunked and a \r\n is encountered in different chunks.")>]
[<Test>]
[<TestCaseSource("readLinesTests")>]
let ``test readLines should return the lines from the input when enumerated one byte at a time``(input, expected:Choice<BS list, BS list>) =
  let actual = enumerate (create input) readLines |> run
  actual |> should equal expected

[<Test>]
[<TestCaseSource("readLinesTests")>]
let ``test readLines should return the lines from the input when chunked``(input, expected:Choice<BS list, BS list>) =
  let actual = enumeratePureNChunk 11 (* Problem is that this is not consistent; try 5 and 10 *) (create input) readLines |> run
  actual |> should equal expected


(* CSV Parser *)

let takeUntilComma = takeUntil ((=) ','B)

[<Test>]
let ``test takeUntilComma should take until the first comma``() =
  let csvSample = BS("blah,blah,blah"B)
  let actual = enumerate csvSample takeUntilComma |> run
  actual |> should equal (BS("blah"B))

let many i =
    let rec inner acc = i >>= check acc
    and check cont bs =
        if ByteString.isEmpty bs then
            Done(cont [], Chunk bs)
        else inner (fun tail -> cont (bs::tail))
    inner id

let readCsvLine = many (takeUntilComma <* drop 1)

[<Test>]
let ``test readCsvLine should take chunks until no commas remain``() =
  let csvSample = BS("blah,blah,blah"B)
  let actual = enumerate csvSample readCsvLine |> run
  actual |> should equal [BS("blah"B);BS("blah"B);BS("blah"B)]

[<Test>]
let ``test readCsvLine should return the empty byte string when that's all it is passed``() =
  let csvSample = ByteString.empty
  let actual = enumerate csvSample readCsvLine |> run
  actual |> should equal ByteString.empty

let many1 i =
    let rec inner acc = i >>= check acc
    and check acc bs =
        if ByteString.isEmpty bs then
            if List.isEmpty acc then
                Error <| failwith "Required at least one match but found none"
            else Done(List.rev acc, Chunk bs)
        else inner (bs::acc)
    inner []

let readCsvLine1 = many1 (takeUntilComma <* drop 1)

[<Test>]
let ``test readCsvLine1 should take chunks until no commas remain``() =
  let csvSample = BS("blah,blah,blah"B)
  let actual = enumerate csvSample readCsvLine1 |> run
  actual |> should equal [BS("blah"B);BS("blah"B);BS("blah"B)]

[<Test>]
let ``test readCsvLine1 should throw when no match is found``() =
  let csvSample = ByteString.empty
  (fun () -> enumerate csvSample readCsvLine1 |> run |> ignore) |> should throw typeof<exn>
