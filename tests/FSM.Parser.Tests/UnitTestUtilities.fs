﻿namespace FSM.Parser.Tests

[<AutoOpen>]
module UnitTestUtilities =
    open CSharp.UnionTypes
    open BrightSword.RoslynWrapper
    open NUnit.Framework

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open Microsoft.CodeAnalysis.CSharp.Syntax

    open FSM.Orleans

    let classes_to_code classes =
        ``compilation unit``
            [
                ``namespace`` "DU.Tests"
                    ``{``
                        [ "System"; "System.Collections" ]
                        classes
                    ``}`` :> MemberDeclarationSyntax
            ]
        |> generateCodeToString

    let class_to_code c = classes_to_code [ c ]

    let text_matches = (mapTuple2 (fixupNL >> trimWS) >> Assert.AreEqual)

    let internal test_codegen t generator expected =
        let actual =
            t
            |> to_class_declaration_internal [ generator ]
            |> class_to_code
        text_matches (expected, actual)

    let internal test_codegen_choice t generator expected =
        let actual =
            t.UnionMembers
            |> List.map ((to_choice_class_internal [ generator ] t) >> class_to_code)
            |> String.concat("\n")
        text_matches (expected, actual)

    let test_codegen_interface t generator expected = 
        let actual = 
            ``compilation unit``
                [
                    t |> to_namespace_builder_internal [ generator ]
                ]
            |> generateCodeToString
        in
        text_matches (expected, actual)
