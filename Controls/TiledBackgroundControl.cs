using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace andecr.Controls;

/// <summary>
/// A custom Avalonia control that fills its background with a tiled image.
/// </summary>
/// <remarks>
/// Tiles are laid out starting from the center of the control and expanding outward
/// in concentric rings traversed counter-clockwise (up → left → down → right in
/// screen coordinates, where Y increases downward).
/// <para>
/// The size of each tile can be set explicitly via <see cref="TileWidth"/> and
/// <see cref="TileHeight"/>, or left as <see cref="TileSize.Auto"/> to use the
/// intrinsic dimensions of <see cref="TileImage"/>.
/// </para>
/// <para>
/// Resolved tile dimensions are cached after the first render pass and invalidated
/// automatically whenever <see cref="TileImage"/>, <see cref="TileWidth"/>, or
/// <see cref="TileHeight"/> changes.
/// </para>
/// </remarks>
public class TiledBackgroundControl : Control
{
    /// <summary>
    /// Cached tile size, computed once on the first render and cleared when any
    /// tile-sizing property changes.
    /// </summary>
    private Size? _resolvedTileSize;

    /// <summary>
    /// Responds to changes in Avalonia styled properties.
    /// Clears <see cref="_resolvedTileSize"/> and schedules a re-render when
    /// <see cref="TileImage"/>, <see cref="TileWidth"/>, or <see cref="TileHeight"/>
    /// is modified; all other property changes are forwarded to the base implementation.
    /// </summary>
    /// <param name="change">Metadata describing the property that changed.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != TileImageProperty &&
            change.Property != TileWidthProperty &&
            change.Property != TileHeightProperty) return;
        _resolvedTileSize = null;
        InvalidateVisual();
    }

    // ── Styled properties ────────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="TileImage"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IImage?> TileImageProperty =
        AvaloniaProperty.Register<TiledBackgroundControl, IImage?>(nameof(TileImage));

    /// <summary>
    /// Identifies the <see cref="TileWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TileSize> TileWidthProperty =
        AvaloniaProperty.Register<TiledBackgroundControl, TileSize>(
            nameof(TileWidth), 
            TileSize.Auto
        );

    /// <summary>
    /// Identifies the <see cref="TileHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TileSize> TileHeightProperty =
        AvaloniaProperty.Register<TiledBackgroundControl, TileSize>(
            nameof(TileHeight), 
            TileSize.Auto
        );

    // ── CLR wrappers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Gets or sets the image used to paint each tile.
    /// When <see langword="null"/> the control renders nothing.
    /// </summary>
    public IImage? TileImage
    {
        get => GetValue(TileImageProperty);
        set => SetValue(TileImageProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of a single tile.
    /// Use <see cref="TileSize.Auto"/> (the default) to inherit the intrinsic width
    /// from <see cref="TileImage"/>; supply an explicit <see cref="TileSize"/> value
    /// to scale or stretch tiles horizontally.
    /// </summary>
    public TileSize TileWidth
    {
        get => GetValue(TileWidthProperty);
        set => SetValue(TileWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of a single tile.
    /// Use <see cref="TileSize.Auto"/> (the default) to inherit the intrinsic height
    /// from <see cref="TileImage"/>; supply an explicit <see cref="TileSize"/> value
    /// to scale or stretch tiles vertically.
    /// </summary>
    public TileSize TileHeight
    {
        get => GetValue(TileHeightProperty);
        set => SetValue(TileHeightProperty, value);
    }

    // ── Static initializer ───────────────────────────────────────────────────────

    static TiledBackgroundControl()
    {
        // Changing TileImage alone is enough to trigger a render pass;
        // width/height changes are handled via OnPropertyChanged → InvalidateVisual().
        AffectsRender<TiledBackgroundControl>(TileImageProperty);
    }

    // ── Rendering ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Renders the tiled background onto the provided <paramref name="context"/>.
    /// </summary>
    /// <remarks>
    /// On the first call (or after a property invalidation) the effective tile size
    /// is resolved by combining <see cref="TileWidth"/> / <see cref="TileHeight"/>
    /// with the intrinsic size of <see cref="TileImage"/> and the current control
    /// bounds. The result is cached in <see cref="_resolvedTileSize"/> for subsequent
    /// frames.
    /// <para>
    /// The centre tile is positioned so that it is centred within the control bounds.
    /// Additional tiles are placed in rings using <see cref="GetCcwSpiral"/> until
    /// all visible area is covered. Rendering is clipped to the control bounds.
    /// </para>
    /// </remarks>
    /// <param name="context">The Avalonia drawing context for the current frame.</param>
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var img = TileImage;
        if (img is null) return;

        var w = Bounds.Width;
        var h = Bounds.Height;

        // Resolve and cache the effective tile size on the first render.
        if (_resolvedTileSize is null)
        {
            var tileW = TileWidth.Resolve(img.Size.Width,  w);
            var tileH = TileHeight.Resolve(img.Size.Height, h);
            _resolvedTileSize = new Size(tileW, tileH);
        }

        var tileWidth  = _resolvedTileSize.Value.Width;
        var tileHeight = _resolvedTileSize.Value.Height;

        if (tileWidth <= 0 || tileHeight <= 0) return;

        // Top-left corner of the centre tile (pixel-aligned).
        var originX = Math.Floor(w / 2 - tileWidth  / 2);
        var originY = Math.Floor(h / 2 - tileHeight / 2);

        // Number of extra tile columns / rows needed to cover each edge.
        var colsLeft  = (int)Math.Ceiling(originX / tileWidth)  + 1;
        var colsRight = (int)Math.Ceiling((w - originX - tileWidth)  / tileWidth)  + 1;
        var rowsUp    = (int)Math.Ceiling(originY / tileHeight) + 1;
        var rowsDown  = (int)Math.Ceiling((h - originY - tileHeight) / tileHeight) + 1;

        var cells = GetCcwSpiral(-colsLeft, colsRight, -rowsUp, rowsDown);

        using var clip = context.PushClip(new Rect(0, 0, w, h));

        foreach (var (col, row) in cells)
        {
            var x = originX + col * tileWidth;
            var y = originY + row * tileHeight;
            context.DrawImage(img, new Rect(x, y, tileWidth, tileHeight));
        }
    }

    // ── Spiral enumeration ───────────────────────────────────────────────────────

    /// <summary>
    /// Enumerates grid cell coordinates in a counter-clockwise outward spiral,
    /// clipped to the specified column / row bounds.
    /// </summary>
    /// <remarks>
    /// The origin cell <c>(0, 0)</c> is yielded first (the centre tile).
    /// Subsequent rings expand outward; within each ring the traversal order is
    /// counter-clockwise in screen coordinates (Y-axis pointing down):
    /// <list type="number">
    ///   <item>Top edge  — left column to right column (east → west in math coords)</item>
    ///   <item>Left edge — top row to bottom row</item>
    ///   <item>Bottom edge — left column to right column</item>
    ///   <item>Right edge — bottom row to top row</item>
    /// </list>
    /// Cells outside the <paramref name="minCol"/>/<paramref name="maxCol"/>/<paramref
    /// name="minRow"/>/<paramref name="maxRow"/> window are silently skipped.
    /// Iteration stops as soon as a full ring produces no in-bounds cells.
    /// </remarks>
    /// <param name="minCol">Minimum (most negative) column index to include.</param>
    /// <param name="maxCol">Maximum (most positive) column index to include.</param>
    /// <param name="minRow">Minimum (most negative) row index to include.</param>
    /// <param name="maxRow">Maximum (most positive) row index to include.</param>
    /// <returns>
    /// A lazily evaluated sequence of <c>(col, row)</c> pairs ordered by the
    /// counter-clockwise spiral traversal described above.
    /// </returns>
    private static IEnumerable<(int col, int row)> GetCcwSpiral(
        int minCol, int maxCol, int minRow, int maxRow)
    {
        yield return (0, 0);

        var ring = 1;
        while (true)
        {
            var any = false;

            // Top edge: row = -ring, col from +ring down to -ring.
            for (var col = ring; col >= -ring; col--)
            {
                if (col < minCol || col > maxCol || -ring < minRow || -ring > maxRow) continue;
                yield return (col, -ring); any = true;
            }
            // Left edge: col = -ring, row from -ring+1 up to +ring.
            for (var row = -ring + 1; row <= ring; row++)
            {
                if (-ring < minCol || -ring > maxCol || row < minRow || row > maxRow) continue;
                yield return (-ring, row); any = true;
            }
            // Bottom edge: row = +ring, col from -ring+1 up to +ring.
            for (var col = -ring + 1; col <= ring; col++)
            {
                if (col < minCol || col > maxCol || ring < minRow || ring > maxRow) continue;
                yield return (col, ring); any = true;
            }
            // Right edge: col = +ring, row from +ring-1 down to -ring+1.
            for (var row = ring - 1; row >= -ring + 1; row--)
            {
                if (ring < minCol || ring > maxCol || row < minRow || row > maxRow) continue;
                yield return (ring, row); any = true;
            }

            if (!any) yield break;
            ring++;
        }
    }
}