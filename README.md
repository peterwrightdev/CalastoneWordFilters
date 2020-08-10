## CalastoneWordFilters
 
 
# Overview

Solution for Calastone Coding assignment for a C# application with associated unit tests to take multiple text filters and apply them to a given string.

Solution reads from a .txt file, and applies up to 3 filters as defined in the coding assignment;

ExcludeCentreVowelFilter excludes words which have a vowel as their middle character(s)

ExcludeShortWordsFilter excludes short words (defaults to less than 3 characters as per spec, parameter can be overwritten by user when running the application)

ExcludeWordsContainingCharFilter excludes words containing a specific character (defaults to 't', can be overwritten by user when running the application).

To run the application, simply run the executable and follow the instructions provided in command line. There are some extra steps I've included for extendability.

# Example to run

Run the executable

When prompted, type filename (including path if file is not already in the same directory as the .exe) and press enter to submit

When prompted, select which filters to apply. Details below provide some more on this, but hopefully the names are fairly self-explanatory. Enter "0 1 2" (without quotes) to run all filters, or any subset of that.

If a parameterised Filter was chosen to include (ExcludeShortWordsFilter or ExcludeWordsContainingCharFilter), then user will be prompted to enter a value if they wish. For ExcludeShortWordsFilter, this will determine the minimum length of the word that can be returned, for ExcludeWordsContainingCharFilter, this will define a character to blacklist. User can just press enter here to take the defaults of 3 and t respectively

Once these have been supplied, the filters selected with the parameters chosen (if applicable) will be applied to the file and resulting words which are not removed by the filters will be returned.

# Details

Expectation of extension of the solution would lie in addition of more Filters, so code has been written to allow for the extension of more Filters to be applied based on classes in the namespace "CalastoneWordFilterer.Filters" and extend the IFilter interface. Simply adding new Filters to this namespace which extend the interface will automatically be used by the rest of the application.

Especially if more Filters were to be added in future, optionally only running some of the filters may be preferable. So I've also included the ability for the user to define which filters to apply. With more time, would have made the messages which show the user what their options are and what they mean a bit clearer.

I noticed that a couple of the Filters defined in the spec seemed appropriate for parametrisation - namely, filters on excluding a specific character (t), or smaller than a specific size (3). As such, I included the functionality for parameterised Filters, which request inputs from users when running the Console App in order to extend that functionality. When no parameters are supplied, the application will default to applying the Filters as per the spec.

Throughout the solution, I have made use of injection to enable unit testing of various components. Notably, the file reader and console are mocked out so that no actual files need be used during testing, and no actual console instance is initiated or used. The retrieval of Filters has been relegated to a Factory pattern, in part for ease of extension, and also to allow testing of components which use Filters without actually requiring actual Filters to be used.

File reading is carried out with a StreamReader, with the expectation that this may need to handle fairly large files, so reading one line into memory at a time rather than the entire file at once would be preferable.

# Closing thoughts

Improvements that could be made with more time;

Parameterization of Filters works based off of ParameterInfo classes, which are not fully mockable. Some rework to this bit to improve testability around here would be of benefit.

Technically, the "word" returned has non-alpha characters stripped out, so in case of words like "she's" the apostrophe would be lost. There would be ways around this, such as returning the nth word from the original file, or some changes around how the word is processed.

Assumptions that were made;

There was some odd grammar in the text received as sample for the test; namely that periods did not have a following space character after ending a sentence. I have assumed that it would be best to use these as separators as well as spaces for now, but in reality would like to confirm this with a stakeholder.
