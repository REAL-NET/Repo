namespace Repo.Visual


/// Type of file which represents visual information
type TypeOfVisual =
    /// Xml file
    | XML = 0

    /// Just image connected to element
    | Image = 1

    /// No file provided
    | NoFile = 2

/// This interface represents information about how element is shown on screen.
type IVisualInfo =
    interface
        /// Address to  file.
        abstract LinkToFile: string with get, set

        /// Type of linked file.
        abstract Type: TypeOfVisual with get, set
    end

/// This interface represents information about how node is shown on screen.
type IVisualNodeInfo =
    interface
        inherit IVisualInfo

        /// Position of node on screen.
        abstract Position: (int * int) option with get, set
    end

// This interface represents information about how edge is shown on screen.
type IVisualEdgeInfo =
    interface
        inherit IVisualInfo

        /// Coordinates of routing points without ends.
        abstract RoutingPoints: (int * int) list with get, set
    end
