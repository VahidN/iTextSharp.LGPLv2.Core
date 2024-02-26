using System.util;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationMgr  manages destination objects for the parser
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public sealed class RtfDestinationMgr
{
    /// <summary>
    ///     String representation of document destination.
    /// </summary>
    public const string DESTINATION_DOCUMENT = "document";

    /// <summary>
    ///     String representation of null destination.
    /// </summary>
    public const string DESTINATION_NULL = "null";

    private const bool _ignoreUnknownDestinations = false;

    /// <summary>
    ///     Destination objects.
    ///     There is only one of each destination.
    /// </summary>
    private static readonly NullValueDictionary<string, RtfDestination> _destinationObjects = new();

    /// <summary>
    ///     CtrlWord :: Destination map object.
    ///     Maps control words to their destinations objects.
    ///     Null destination is a special destination used for
    ///     discarding unwanted data. This is primarily used when
    ///     skipping groups, binary data or unwanted/unknown data.
    /// </summary>
    private static readonly NullValueDictionary<string, RtfDestination> _destinations = new();

    /// <summary>
    ///     Destinations
    /// </summary>
    private static RtfDestinationMgr _instance;

    private static RtfParser _rtfParser;

    /// <summary>
    ///     Hidden default constructor becuase
    /// </summary>
    private RtfDestinationMgr()
    {
    }

    public static bool AddDestination(string destination, object[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (_destinations.ContainsKey(destination))
        {
            return true;
        }

        var thisClass = $"iTextSharp.text.rtf.parser.destinations.{(string)args[0]}";

        if (thisClass.IndexOf("RtfDestinationNull", StringComparison.Ordinal) >= 0)
        {
            _destinations[destination] = RtfDestinationNull.GetInstance();

            return true;
        }

        Type type = null;

        try
        {
            type = Type.GetType(thisClass);
        }
        catch
        {
            return false;
        }

        if (type == null)
        {
            return false;
        }

        RtfDestination c = null;

        if (_destinationObjects.TryGetValue(type.Name, out var value))
        {
            c = value;
        }
        else
        {
            try
            {
                c = (RtfDestination)Activator.CreateInstance(type);
            }
            catch
            {
                return false;
            }
        }

        c.SetParser(_rtfParser);

        if (type == typeof(RtfDestinationInfo))
        {
            ((RtfDestinationInfo)c).SetElementName(destination);
        }

        if (type == typeof(RtfDestinationStylesheetTable))
        {
            ((RtfDestinationStylesheetTable)c).SetElementName(destination);
            ((RtfDestinationStylesheetTable)c).SetType((string)args[1]);
        }

        _destinations[destination] = c;
        _destinationObjects[type.Name] = c;

        return true;
    }

    /// <summary>
    ///     Adds a  RtfDestinationListener  to the appropriate  RtfDestination .
    ///     the new RtfDestinationListener.
    /// </summary>
    /// <param name="destination">the destination string for the listener</param>
    /// <param name="listener"></param>
    public static bool AddListener(string destination, IRtfDestinationListener listener)
    {
        var dest = GetDestination(destination);

        if (dest != null)
        {
            return RtfDestination.AddListener(listener);
        }

        return false;
    }

    public static RtfDestination GetDestination(string destination)
    {
        RtfDestination dest;
        dest = _destinations.TryGetValue(destination, out var value) ? value : _destinations[DESTINATION_DOCUMENT];

        dest.SetParser(_rtfParser);

        return dest;
    }

    public static RtfDestinationMgr GetInstance()
    {
        lock (_destinations)
        {
            if (_instance == null)
            {
                _instance = new RtfDestinationMgr();

                // 2 required destinations for all documents
                AddDestination(DESTINATION_DOCUMENT, new object[]
                {
                    "RtfDestinationDocument", ""
                });

                AddDestination(DESTINATION_NULL, new object[]
                {
                    "RtfDestinationNull", ""
                });
            }

            return _instance;
        }
    }

    public static RtfDestinationMgr GetInstance(RtfParser parser)
    {
        lock (_destinations)
        {
            SetParser(parser);

            if (_instance == null)
            {
                _instance = new RtfDestinationMgr();

                // 2 required destinations for all documents
                AddDestination(DESTINATION_DOCUMENT, new object[]
                {
                    "RtfDestinationDocument", ""
                });

                AddDestination(DESTINATION_NULL, new object[]
                {
                    "RtfDestinationNull", ""
                });
            }

            return _instance;
        }
    }

    /// <summary>
    ///     listener methods
    /// </summary>
    /// <summary>
    ///     Removes a  RtfDestinationListener  from the appropriate  RtfDestination .
    ///     the RtfCtrlWordListener that has to be removed.
    /// </summary>
    /// <param name="destination">the destination string for the listener</param>
    /// <param name="listener"></param>
    public static bool RemoveListener(string destination, IRtfDestinationListener listener)
    {
        var dest = GetDestination(destination);

        if (dest != null)
        {
            return RtfDestination.RemoveListener(listener);
        }

        return false;
    }

    public static void SetParser(RtfParser parser)
        => _rtfParser = parser;
}