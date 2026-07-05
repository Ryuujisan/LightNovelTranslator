// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using LightNovelTranslator.Docx;
using LightNovelTranslator.Ollama;
/*
var translator = new OllamaTranslator();
var sw = Stopwatch.StartNew();
var text = """
                                                 The classroom was empty after school.
                                                 
                                                 Golden light streamed through the windows, painting long shadows across the floor.
                                                 
                                                 Narumi sat alone at her desk, staring at the notebook she had opened several minutes ago.
                                                 
                                                 The page was still blank.
                                                 
                                                 She let out a quiet sigh.
                                                 
                                                 No matter how hard she tried, she couldn't focus.
                                                 
                                                 The events of the previous night refused to leave her mind.
                                                 
                                                 A sudden knock broke the silence.
                                                 
                                                 Before she could answer, the classroom door slowly opened.
                                                 
                                                 "Still here?"
                                                 
                                                 Narumi looked up.
                                                 
                                                 A silver-haired girl stood in the doorway with a faint smile on her face.
                                                 
                                                 "Lux..."
                                                 
                                                 "I thought you would have gone home already."
                                                 
                                                 "So did I."
                                                 
                                                 The girl stepped into the classroom and pulled a chair closer.
                                                 
                                                 For a moment, neither of them spoke.
                                                 
                                                 The peaceful silence wasn't uncomfortable.
                                                 
                                                 If anything, it felt strangely reassuring.
                                                 
                                                 Lux rested her chin on her hand and watched Narumi carefully.
                                                 
                                                 "You're worried about something."
                                                 
                                                 It wasn't a question.
                                                 
                                                 Narumi lowered her gaze.
                                                 
                                                 "...Is it that obvious?"
                                                 
                                                 "A little."
                                                 
                                                 The silver-haired girl laughed softly.
                                                 
                                                 Narumi couldn't help but smile in return.
                                                 
                                                 For the first time that day, the weight on her chest felt slightly lighter.
                                                 """;
var translated = await translator.TranslateAsync(text);

sw.Stop();

Console.WriteLine($"Chars: {text.Length}");
Console.WriteLine($"Time: {sw.Elapsed}");
Console.WriteLine($"Chars/sec: {text.Length / sw.Elapsed.TotalSeconds:F2}");
Console.WriteLine(translated);*/
var reader = new DocxDocumentReader();
var translator = new DocxTranslator(
    new OllamaTranslator());

var writer = new DocxDocumentWriter();

var inputFile = "test.docx";
var outputFile = "test_translated.docx";

var document =
    await reader.ReadAsync(inputFile);

var translatedDocument =
    await translator.TranslateAsync(document);

await writer.WriteAsync(
    inputFile,
    outputFile,
    translatedDocument);

Console.WriteLine("Done.");
