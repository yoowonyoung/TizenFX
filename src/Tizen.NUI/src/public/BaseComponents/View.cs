/*
 * Copyright(c) 2019 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Tizen.NUI.Binding;

namespace Tizen.NUI.BaseComponents
{
    /// <summary>
    /// View is the base class for all views.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public partial class View : Container, IResourcesProvider
    {
        /// <summary>
        /// Flag to indicate if layout set explicitly via API call or View was automatically given a Layout.
        /// </summary>
        /// This will be public opened in tizen_5.5 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool layoutSet = false;

        /// <summary>
        /// Flag to allow Layouting to be disabled for Views.
        /// Once a View has a Layout set then any children added to Views from then on will receive
        /// automatic Layouts.
        /// </summary>
        /// This will be public opened in tizen_5.5 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool layoutingDisabled{get; set;} = true;

        private global::System.Runtime.InteropServices.HandleRef swigCPtr;
        private LayoutItem _layout; // Exclusive layout assigned to this View.

        // List of transitions paired with the condition that uses the transition.
        private Dictionary<TransitionCondition, TransitionList> _layoutTransitions;
        private int _widthPolicy = LayoutParamPolicies.WrapContent; // Layout width policy
        private int _heightPolicy = LayoutParamPolicies.WrapContent; // Layout height policy
        private float _weight = 0.0f; // Weighting of child View in a Layout
        private MeasureSpecification _measureSpecificationWidth; // Layout width and internal Mode
        private MeasureSpecification _measureSpecificationHeight; // Layout height and internal Mode
        private bool _backgroundImageSynchronosLoading = false;
        private Dictionary<string, Transition> transDictionary = new Dictionary<string, Transition>();
        private string[] transitionNames;

        /// <summary>
        /// Creates a new instance of a view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public View() : this(Interop.View.View_New(), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Create a new instance of a View with setting the status of shown or hidden.
        /// </summary>
        /// <param name="shown">false : Not displayed (hidden), true : displayed (shown)</param>
        /// This will be public opened in next release of tizen after ACR done. Before ACR, it is used as HiddenAPI (InhouseAPI).
        [EditorBrowsable(EditorBrowsableState.Never)]
        public View(bool shown) : this(Interop.View.View_New(), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            SetVisible(shown);
        }

        internal View(View uiControl, bool shown = true) : this(Interop.View.new_View__SWIG_1(View.getCPtr(uiControl)), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            if(!shown)
            {
                SetVisible(false);
            }
        }

        internal View(global::System.IntPtr cPtr, bool cMemoryOwn, bool shown = true) : base(Interop.View.View_SWIGUpcast(cPtr), cMemoryOwn)
        {
            swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
            if (HasBody())
            {
                PositionUsesPivotPoint = false;
            }

            _onWindowSendEventCallback = SendViewAddedEventToWindow;
            this.OnWindowSignal().Connect(_onWindowSendEventCallback);

            if (!shown)
            {
                SetVisible(false);
            }
        }

        internal View(ViewImpl implementation, bool shown = true) : this(Interop.View.new_View__SWIG_2(ViewImpl.getCPtr(implementation)), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();

            if (!shown)
            {
                SetVisible(false);
            }
        }

        /// This will be public opened in tizen_5.0 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsResourcesCreated
        {
            get
            {
                return Application.Current.IsResourcesCreated;
            }
        }

        /// This will be public opened in tizen_5.0 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ResourceDictionary XamlResources
        {
            get
            {
                return Application.Current.XamlResources;
            }
            set
            {
                Application.Current.XamlResources = value;
            }
        }

        /// <summary>
        /// The StyleName, type string.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public string StyleName
        {
            get
            {
                return (string)GetValue(StyleNameProperty);
            }
            set
            {
                SetValue(StyleNameProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The mutually exclusive with "backgroundImage" and "background" type Vector4.
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.BackgroundColor.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Color BackgroundColor
        {
            get
            {
                Color temp = (Color)GetValue(BackgroundColorProperty);
                return new Color(OnBackgroundColorChanged, temp.R, temp.G, temp.B, temp.A);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The mutually exclusive with "backgroundColor" and "background" type Map.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public string BackgroundImage
        {
            get
            {
                return (string)GetValue(BackgroundImageProperty);
            }
            set
            {
                SetValue(BackgroundImageProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The background of view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Tizen.NUI.PropertyMap Background
        {
            get
            {
                return (PropertyMap)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The current state of the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public States State
        {
            get
            {
                return (States)GetValue(StateProperty);
            }
            set
            {
                SetValue(StateProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The current sub state of the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public States SubState
        {
            get
            {
                return (States)GetValue(SubStateProperty);
            }
            set
            {
                SetValue(SubStateProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Displays a tooltip
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Tizen.NUI.PropertyMap Tooltip
        {
            get
            {
                return (PropertyMap)GetValue(TooltipProperty);
            }
            set
            {
                SetValue(TooltipProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Displays a tooltip as a text.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public string TooltipText
        {
            set
            {
                SetProperty(View.Property.TOOLTIP, new Tizen.NUI.PropertyValue(value));
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The Child property of FlexContainer.<br />
        /// The proportion of the free space in the container, the flex item will receive.<br />
        /// If all items in the container set this property, their sizes will be proportional to the specified flex factor.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public float Flex
        {
            get
            {
                return (float)GetValue(FlexProperty);
            }
            set
            {
                SetValue(FlexProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The Child property of FlexContainer.<br />
        /// The alignment of the flex item along the cross axis, which, if set, overides the default alignment for all items in the container.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int AlignSelf
        {
            get
            {
                return (int)GetValue(AlignSelfProperty);
            }
            set
            {
                SetValue(AlignSelfProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The Child property of FlexContainer.<br />
        /// The space around the flex item.<br />
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.FlexMargin.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Vector4 FlexMargin
        {
            get
            {
                Vector4 temp = (Vector4)GetValue(FlexMarginProperty);
                return new Vector4(OnFlexMarginChanged, temp.X, temp.Y, temp.Z, temp.W);
            }
            set
            {
                SetValue(FlexMarginProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The top-left cell this child occupies, if not set, the first available cell is used.
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.CellIndex.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Vector2 CellIndex
        {
            get
            {
                Vector2 temp = (Vector2)GetValue(CellIndexProperty);
                return new Vector2(OnCellIndexChanged, temp.X, temp.Y);
            }
            set
            {
                SetValue(CellIndexProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The number of rows this child occupies, if not set, the default value is 1.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public float RowSpan
        {
            get
            {
                return (float)GetValue(RowSpanProperty);
            }
            set
            {
                SetValue(RowSpanProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The number of columns this child occupies, if not set, the default value is 1.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public float ColumnSpan
        {
            get
            {
                return (float)GetValue(ColumnSpanProperty);
            }
            set
            {
                SetValue(ColumnSpanProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The horizontal alignment of this child inside the cells, if not set, the default value is 'left'.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Tizen.NUI.HorizontalAlignmentType CellHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignmentType)GetValue(CellHorizontalAlignmentProperty);
            }
            set
            {
                SetValue(CellHorizontalAlignmentProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The vertical alignment of this child inside the cells, if not set, the default value is 'top'.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Tizen.NUI.VerticalAlignmentType CellVerticalAlignment
        {
            get
            {
                return (VerticalAlignmentType)GetValue(CellVerticalAlignmentProperty);
            }
            set
            {
                SetValue(CellVerticalAlignmentProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The left focusable view.<br />
        /// This will return null if not set.<br />
        /// This will also return null if the specified left focusable view is not on a window.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public View LeftFocusableView
        {
            // As native side will be only storing IDs so need a logic to convert View to ID and vice-versa.
            get
            {
                return (View)GetValue(LeftFocusableViewProperty);
            }
            set
            {
                SetValue(LeftFocusableViewProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The right focusable view.<br />
        /// This will return null if not set.<br />
        /// This will also return null if the specified right focusable view is not on a window.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public View RightFocusableView
        {
            // As native side will be only storing IDs so need a logic to convert View to ID and vice-versa.
            get
            {
                return (View)GetValue(RightFocusableViewProperty);
            }
            set
            {
                SetValue(RightFocusableViewProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The up focusable view.<br />
        /// This will return null if not set.<br />
        /// This will also return null if the specified up focusable view is not on a window.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public View UpFocusableView
        {
            // As native side will be only storing IDs so need a logic to convert View to ID and vice-versa.
            get
            {
                return (View)GetValue(UpFocusableViewProperty);
            }
            set
            {
                SetValue(UpFocusableViewProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// The down focusable view.<br />
        /// This will return null if not set.<br />
        /// This will also return null if the specified down focusable view is not on a window.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public View DownFocusableView
        {
            // As native side will be only storing IDs so need a logic to convert View to ID and vice-versa.
            get
            {
                return (View)GetValue(DownFocusableViewProperty);
            }
            set
            {
                SetValue(DownFocusableViewProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the view should be focusable by keyboard navigation.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool Focusable
        {
            set
            {
                SetValue(FocusableProperty, value);
                NotifyPropertyChanged();
            }
            get
            {
                return (bool)GetValue(FocusableProperty);
            }
        }

        /// <summary>
        ///  Retrieves the position of the view.<br />
        ///  The coordinates are relative to the view's parent.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Position CurrentPosition
        {
            get
            {
                return GetCurrentPosition();
            }
        }

        /// <summary>
        /// Sets the size of a view for the width and the height.<br />
        /// Geometry can be scaled to fit within this area.<br />
        /// This does not interfere with the view's scale factor.<br />
        /// The views default depth is the minimum of width and height.<br />
        /// </summary>
        /// <remarks>
        /// This NUI object (Size2D) typed property can be configured by multiple cascade setting. <br />
        /// For example, this code ( view.Size2D.Width = 100; view.Size2D.Height = 100; ) is equivalent to this ( view.Size2D = new Size2D(100, 100); ). <br />
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Size2D Size2D
        {
            get
            {
                Size2D temp = (Size2D)GetValue(Size2DProperty);
                return new Size2D(OnSize2DChanged, temp.Width, temp.Height);
            }
            set
            {
                SetValue(Size2DProperty, value);
                // Set Specification so when layouts measure this View it matches the value set here.
                // All Views are currently Layouts.
                MeasureSpecificationWidth = new MeasureSpecification(new LayoutLength(value.Width), MeasureSpecification.ModeType.Exactly);
                MeasureSpecificationHeight = new MeasureSpecification(new LayoutLength(value.Height), MeasureSpecification.ModeType.Exactly);
                _widthPolicy = value.Width;
                _heightPolicy = value.Height;
                _layout?.RequestLayout();
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///  Retrieves the size of the view.<br />
        ///  The coordinates are relative to the view's parent.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Size2D CurrentSize
        {
            get
            {
                return GetCurrentSize();
            }
        }

        /// <summary>
        /// Retrieves and sets the view's opacity.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public float Opacity
        {
            get
            {
                return (float)GetValue(OpacityProperty);
            }
            set
            {
                SetValue(OpacityProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets the position of the view for X and Y.<br />
        /// By default, sets the position vector between the parent origin and the pivot point (default).<br />
        /// If the position inheritance is disabled, sets the world position.<br />
        /// </summary>
        /// <remarks>
        /// This NUI object (Position2D) typed property can be configured by multiple cascade setting. <br />
        /// For example, this code ( view.Position2D.X = 100; view.Position2D.Y = 100; ) is equivalent to this ( view.Position2D = new Position2D(100, 100); ). <br />
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Position2D Position2D
        {
            get
            {
                Position2D temp = (Position2D)GetValue(Position2DProperty);
                return new Position2D(OnPosition2DChanged, temp.X, temp.Y);
            }
            set
            {
                SetValue(Position2DProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Retrieves the screen postion of the view.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Vector2 ScreenPosition
        {
            get
            {
                Vector2 temp = new Vector2(0.0f, 0.0f);
                GetProperty(View.Property.SCREEN_POSITION).Get(temp);
                return temp;
            }
        }

        /// <summary>
        /// Determines whether the pivot point should be used to determine the position of the view.
        /// This is true by default.
        /// </summary>
        /// <remarks>If false, then the top-left of the view is used for the position.
        /// Setting this to false will allow scaling or rotation around the pivot point without affecting the view's position.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public bool PositionUsesPivotPoint
        {
            get
            {
                return (bool)GetValue(PositionUsesPivotPointProperty);
            }
            set
            {
                SetValue(PositionUsesPivotPointProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Deprecated in API5; Will be removed in API8. Please use PositionUsesPivotPoint instead!
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        [Obsolete("Deprecated in API5; Will be removed in API8. Please use PositionUsesPivotPoint instead! " +
            "Like: " +
            "View view = new View(); " +
            "view.PivotPoint = PivotPoint.Center; " +
            "view.PositionUsesPivotPoint = true;" +
            " Deprecated in API5: Will be removed in API8")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool PositionUsesAnchorPoint
        {
            get
            {
                bool temp = false;
                GetProperty(View.Property.POSITION_USES_ANCHOR_POINT).Get(out temp);
                return temp;
            }
            set
            {
                SetProperty(View.Property.POSITION_USES_ANCHOR_POINT, new Tizen.NUI.PropertyValue(value));
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Queries whether the view is connected to the stage.<br />
        /// When a view is connected, it will be directly or indirectly parented to the root view.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool IsOnWindow
        {
            get
            {
                return OnWindow();
            }
        }

        /// <summary>
        /// Gets the depth in the hierarchy for the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int HierarchyDepth
        {
            get
            {
                return GetHierarchyDepth();
            }
        }

        /// <summary>
        /// Sets the sibling order of the view so the depth position can be defined within the same parent.
        /// </summary>
        /// <remarks>
        /// Note the initial value is 0. SiblingOrder should be bigger than 0 or equal to 0.
        /// Raise, Lower, RaiseToTop, LowerToBottom, RaiseAbove, and LowerBelow will override the sibling order.
        /// The values set by this property will likely change.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public int SiblingOrder
        {
            get
            {
                return (int)GetValue(SiblingOrderProperty);
            }
            set
            {
                SetValue(SiblingOrderProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Returns the natural size of the view.
        /// </summary>
        /// <remarks>
        /// Deriving classes stipulate the natural size and by default a view has a zero natural size.
        /// </remarks>
        /// <since_tizen> 5 </since_tizen>
        public Vector3 NaturalSize
        {
            get
            {
                Vector3 ret = new Vector3(Interop.Actor.Actor_GetNaturalSize(swigCPtr), true);
                if (NDalicPINVOKE.SWIGPendingException.Pending)
                    throw NDalicPINVOKE.SWIGPendingException.Retrieve();
                return ret;
            }
        }

        /// <summary>
        /// Returns the natural size (Size2D) of the view.
        /// </summary>
        /// <remarks>
        /// Deriving classes stipulate the natural size and by default a view has a zero natural size.
        /// </remarks>
        /// <since_tizen> 4 </since_tizen>
        public Size2D NaturalSize2D
        {
            get
            {
                Vector3 temp = new Vector3(Interop.Actor.Actor_GetNaturalSize(swigCPtr), true);
                if (NDalicPINVOKE.SWIGPendingException.Pending)
                    throw NDalicPINVOKE.SWIGPendingException.Retrieve();

                return new Size2D((int)temp.Width, (int)temp.Height);
            }
        }

        /// <summary>
        /// Gets or sets the origin of a view within its parent's area.<br />
        /// This is expressed in unit coordinates, such that (0.0, 0.0, 0.5) is the top-left corner of the parent, and (1.0, 1.0, 0.5) is the bottom-right corner.<br />
        /// The default parent-origin is ParentOrigin.TopLeft (0.0, 0.0, 0.5).<br />
        /// A view's position is the distance between this origin and the view's anchor-point.<br />
        /// </summary>
        /// <pre>The view has been initialized.</pre>
        /// <since_tizen> 3 </since_tizen>
        public Position ParentOrigin
        {
            get
            {
                Position tmp = (Position)GetValue(ParentOriginProperty);
                return new Position(OnParentOriginChanged, tmp.X, tmp.Y, tmp.Z);
            }
            set
            {
                SetValue(ParentOriginProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the anchor-point of a view.<br />
        /// This is expressed in unit coordinates, such that (0.0, 0.0, 0.5) is the top-left corner of the view, and (1.0, 1.0, 0.5) is the bottom-right corner.<br />
        /// The default pivot point is PivotPoint.Center (0.5, 0.5, 0.5).<br />
        /// A view position is the distance between its parent-origin and this anchor-point.<br />
        /// A view's orientation is the rotation from its default orientation, the rotation is centered around its anchor-point.<br />
        /// <pre>The view has been initialized.</pre>
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.PivotPoint.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Position PivotPoint
        {
            get
            {
                Position tmp = (Position)GetValue(PivotPointProperty);
                return new Position(OnPivotPointChanged, tmp.X, tmp.Y, tmp.Z);
            }
            set
            {
                SetValue(PivotPointProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size width of the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float SizeWidth
        {
            get
            {
                return (float)GetValue(SizeWidthProperty);
            }
            set
            {
                SetValue(SizeWidthProperty, value);
                WidthSpecification = (int)Math.Ceiling(value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size height of the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float SizeHeight
        {
            get
            {
                return (float)GetValue(SizeHeightProperty);
            }
            set
            {
                SetValue(SizeHeightProperty, value);
                HeightSpecification = (int)Math.Ceiling(value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the position of the view.<br />
        /// By default, sets the position vector between the parent origin and pivot point (default).<br />
        /// If the position inheritance is disabled, sets the world position.<br />
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// The property cascade chaining set is possible. For example, this (view.Position.X = 1.0f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Position Position
        {
            get
            {
                Position tmp = (Position)GetValue(PositionProperty);
                return new Position(OnPositionChanged, tmp.X, tmp.Y, tmp.Z);
            }
            set
            {
                SetValue(PositionProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the position X of the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float PositionX
        {
            get
            {
                return (float)GetValue(PositionXProperty);
            }
            set
            {
                SetValue(PositionXProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the position Y of the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float PositionY
        {
            get
            {
                return (float)GetValue(PositionYProperty);
            }
            set
            {
                SetValue(PositionYProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the position Z of the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float PositionZ
        {
            get
            {
                return (float)GetValue(PositionZProperty);
            }
            set
            {
                SetValue(PositionZProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the world position of the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Vector3 WorldPosition
        {
            get
            {
                Vector3 temp = new Vector3(0.0f, 0.0f, 0.0f);
                GetProperty(View.Property.WORLD_POSITION).Get(temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the view.<br />
        /// The view's orientation is the rotation from its default orientation, and the rotation is centered around its anchor-point.<br />
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is an asynchronous method.
        /// </para>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Rotation Orientation
        {
            get
            {
                return (Rotation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the world orientation of the view.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Rotation WorldOrientation
        {
            get
            {
                Rotation temp = new Rotation();
                GetProperty(View.Property.WORLD_ORIENTATION).Get(temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets or sets the scale factor applied to the view.<br />
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// The property cascade chaining set is possible. For example, this (view.Scale.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Vector3 Scale
        {
            get
            {
                Vector3 temp = (Vector3)GetValue(ScaleProperty);
                return new Vector3(OnScaleChanged, temp.X, temp.Y, temp.Z);
            }
            set
            {
                SetValue(ScaleProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the scale X factor applied to the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float ScaleX
        {
            get
            {
                return (float)GetValue(ScaleXProperty);
            }
            set
            {
                SetValue(ScaleXProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the scale Y factor applied to the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float ScaleY
        {
            get
            {
                return (float)GetValue(ScaleYProperty);
            }
            set
            {
                SetValue(ScaleYProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the scale Z factor applied to the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public float ScaleZ
        {
            get
            {
                return (float)GetValue(ScaleZProperty);
            }
            set
            {
                SetValue(ScaleZProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the world scale of the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Vector3 WorldScale
        {
            get
            {
                Vector3 temp = new Vector3(0.0f, 0.0f, 0.0f);
                GetProperty(View.Property.WORLD_SCALE).Get(temp);
                return temp;
            }
        }

        /// <summary>
        /// Retrieves the visibility flag of the view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the view is not visible, then the view and its children will not be rendered.
        /// This is regardless of the individual visibility values of the children, i.e., the view will only be rendered if all of its parents have visibility set to true.
        /// </para>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public bool Visibility
        {
            get
            {
                bool temp = false;
                GetProperty(View.Property.VISIBLE).Get(out temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets the view's world color.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Vector4 WorldColor
        {
            get
            {
                Vector4 temp = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                GetProperty(View.Property.WORLD_COLOR).Get(temp);
                return temp;
            }
        }

        /// <summary>
        /// Gets or sets the view's name.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public string Name
        {
            get
            {
                return (string)GetValue(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Get the number of children held by the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public new uint ChildCount
        {
            get
            {
                return GetChildCount();
            }
        }

        /// <summary>
        /// Gets the view's ID.
        /// Readonly
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public uint ID
        {
            get
            {
                return GetId();
            }
        }

        /// <summary>
        /// Gets or sets the status of whether the view should emit touch or hover signals.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool Sensitive
        {
            get
            {
                return (bool)GetValue(SensitiveProperty);
            }
            set
            {
                SetValue(SensitiveProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status of whether the view should receive a notification when touch or hover motion events leave the boundary of the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool LeaveRequired
        {
            get
            {
                return (bool)GetValue(LeaveRequiredProperty);
            }
            set
            {
                SetValue(LeaveRequiredProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status of whether a child view inherits it's parent's orientation.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool InheritOrientation
        {
            get
            {
                return (bool)GetValue(InheritOrientationProperty);
            }
            set
            {
                SetValue(InheritOrientationProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status of whether a child view inherits it's parent's scale.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool InheritScale
        {
            get
            {
                return (bool)GetValue(InheritScaleProperty);
            }
            set
            {
                SetValue(InheritScaleProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status of how the view and its children should be drawn.<br />
        /// Not all views are renderable, but DrawMode can be inherited from any view.<br />
        /// If an object is in a 3D layer, it will be depth-tested against other objects in the world, i.e., it may be obscured if other objects are in front.<br />
        /// If DrawMode.Overlay2D is used, the view and its children will be drawn as a 2D overlay.<br />
        /// Overlay views are drawn in a separate pass, after all non-overlay views within the layer.<br />
        /// For overlay views, the drawing order is with respect to tree levels of views, and depth-testing will not be used.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public DrawModeType DrawMode
        {
            get
            {
                return (DrawModeType)GetValue(DrawModeProperty);
            }
            set
            {
                SetValue(DrawModeProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the relative to parent size factor of the view.<br />
        /// This factor is only used when ResizePolicyType is set to either: ResizePolicyType.SizeRelativeToParent or ResizePolicyType.SizeFixedOffsetFromParent.<br />
        /// This view's size is set to the view's size multiplied by or added to this factor, depending on ResizePolicyType.<br />
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.DecorationBoundingBox.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Vector3 SizeModeFactor
        {
            get
            {
                Vector3 temp = (Vector3)GetValue(SizeModeFactorProperty);
                return new Vector3(OnSizeModeFactorChanged, temp.X, temp.Y, temp.Z);
            }
            set
            {
                SetValue(SizeModeFactorProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the width resize policy to be used.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public ResizePolicyType WidthResizePolicy
        {
            get
            {
                return (ResizePolicyType)GetValue(WidthResizePolicyProperty);
            }
            set
            {
                SetValue(WidthResizePolicyProperty, value);
                // Match ResizePolicy to new Layouting.
                // Parent relative policies can not be mapped at this point as parent size unknown.
                switch (value)
                {
                    case ResizePolicyType.UseNaturalSize:
                    {
                        WidthSpecification = LayoutParamPolicies.WrapContent;
                        break;
                    }
                    case ResizePolicyType.FillToParent:
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent;
                        break;
                    }
                    case ResizePolicyType.FitToChildren:
                    {
                        WidthSpecification = LayoutParamPolicies.WrapContent;
                        break;
                    }
                    default:
                        break;
                }
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the height resize policy to be used.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public ResizePolicyType HeightResizePolicy
        {
            get
            {
                return (ResizePolicyType)GetValue(HeightResizePolicyProperty);
            }
            set
            {
                SetValue(HeightResizePolicyProperty, value);
                // Match ResizePolicy to new Layouting.
                // Parent relative policies can not be mapped at this point as parent size unknown.
                switch (value)
                {
                    case ResizePolicyType.UseNaturalSize:
                    {
                        HeightSpecification = LayoutParamPolicies.WrapContent;
                        break;
                    }
                    case ResizePolicyType.FillToParent:
                    {
                        HeightSpecification = LayoutParamPolicies.MatchParent;
                        break;
                    }
                    case ResizePolicyType.FitToChildren:
                    {
                        HeightSpecification = LayoutParamPolicies.WrapContent;
                        break;
                    }
                    default:
                        break;
                }
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the policy to use when setting size with size negotiation.<br />
        /// Defaults to SizeScalePolicyType.UseSizeSet.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public SizeScalePolicyType SizeScalePolicy
        {
            get
            {
                return (SizeScalePolicyType)GetValue(SizeScalePolicyProperty);
            }
            set
            {
                SetValue(SizeScalePolicyProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///  Gets or sets the status of whether the width size is dependent on the height size.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool WidthForHeight
        {
            get
            {
                return (bool)GetValue(WidthForHeightProperty);
            }
            set
            {
                SetValue(WidthForHeightProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status of whether the height size is dependent on the width size.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool HeightForWidth
        {
            get
            {
                return (bool)GetValue(HeightForWidthProperty);
            }
            set
            {
                SetValue(HeightForWidthProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the padding for use in layout.
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.Padding.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 5 </since_tizen>
        public Extents Padding
        {
            get
            {
                // If View has a Layout then padding in stored in the base Layout class
                if (Layout !=null)
                {
                    return Layout.Padding;
                }
                else
                {
                    Extents temp = (Extents)GetValue(PaddingProperty);
                    return new Extents(OnPaddingChanged, temp.Start, temp.End, temp.Top, temp.Bottom);
                }
                // Two return points to prevent creating a zeroed Extent native object before assignment
            }
            set
            {
                Extents padding = value;
                if (Layout !=null)
                {
                    // Layout set so store Padding in LayoutItem instead of in View.
                    // If View stores the Padding value then Legacy Size Negotiation will overwrite
                    // the position and sizes measure in the Layouting.
                    Layout.Padding = value;
                    // If Layout is a LayoutItem then it could be a View that handles it's own padding.
                    // Let the View keeps it's padding.  Still store Padding in Layout to reduce code paths.
                    if( Layout.GetType() != typeof(LayoutItem)) // If a Layout container of some kind.
                    {
                        padding =  new Extents(0,0,0,0); // Reset value stored in View.
                    }
                    _layout?.RequestLayout();
                }
                SetValue(PaddingProperty, padding);
                NotifyPropertyChanged();
                _layout?.RequestLayout();
            }
        }

        /// <summary>
        /// Gets or sets the minimum size the view can be assigned in size negotiation.
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.MinimumSize.Width = 1;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Size2D MinimumSize
        {
            get
            {
                Size2D tmp = (Size2D)GetValue(MinimumSizeProperty);
                return new Size2D(OnMinimumSizeChanged, tmp.Width, tmp.Height);
            }
            set
            {
                if (_layout != null)
                {
                    // Note: it only works if minimum size is >= than natural size.
                    // To force the size it should be done through the width&height spec or Size2D.
                    _layout.MinimumWidth = new Tizen.NUI.LayoutLength(value.Width);
                    _layout.MinimumHeight = new Tizen.NUI.LayoutLength(value.Height);
                    _layout.RequestLayout();
                }
                SetValue(MinimumSizeProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the maximum size the view can be assigned in size negotiation.
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.MaximumSize.Width = 1;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public Size2D MaximumSize
        {
            get
            {
                Size2D tmp = (Size2D)GetValue(MaximumSizeProperty);
                return new Size2D(OnMaximumSizeChanged, tmp.Width, tmp.Height);
            }
            set
            {
                // We don't have Layout.Maximum(Width|Height) so we cannot apply it to layout.
                // MATCH_PARENT spec + parent container size can be used to limit
                if (_layout != null)
                {
                    // Note: it only works if minimum size is >= than natural size.
                    // To force the size it should be done through the width&height spec or Size2D.
                    _layout.MinimumHeight = new Tizen.NUI.LayoutLength(value.Width);
                    _layout.MinimumWidth = new Tizen.NUI.LayoutLength(value.Height);
                    _layout.RequestLayout();
                }
                SetValue(MaximumSizeProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether a child view inherits it's parent's position.<br />
        /// Default is to inherit.<br />
        /// Switching this off means that using position sets the view's world position, i.e., translates from the world origin (0,0,0) to the pivot point of the view.<br />
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool InheritPosition
        {
            get
            {
                return (bool)GetValue(InheritPositionProperty);
            }
            set
            {
                SetValue(InheritPositionProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the clipping behavior (mode) of it's children.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public ClippingModeType ClippingMode
        {
            get
            {
                return (ClippingModeType)GetValue(ClippingModeProperty);
            }
            set
            {
                SetValue(ClippingModeProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the number of renderers held by the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public uint RendererCount
        {
            get
            {
                return GetRendererCount();
            }
        }

        /// <summary>
        /// Deprecated in API5; Will be removed in API8. Please use PivotPoint instead!
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.AnchorPoint.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        [Obsolete("Deprecated in API5; Will be removed in API8. Please use PivotPoint instead! " +
            "Like: " +
            "View view = new View(); " +
            "view.PivotPoint = PivotPoint.Center; " +
            "view.PositionUsesPivotPoint = true;")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Position AnchorPoint
        {
            get
            {
                Position temp = new Position(0.0f, 0.0f, 0.0f);
                GetProperty(View.Property.ANCHOR_POINT).Get(temp);
                return new Position(OnAnchorPointChanged, temp.X, temp.Y, temp.Z);
            }
            set
            {
                SetProperty(View.Property.ANCHOR_POINT, new Tizen.NUI.PropertyValue(value));
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets the size of a view for the width, the height and the depth.<br />
        /// Geometry can be scaled to fit within this area.<br />
        /// This does not interfere with the view's scale factor.<br />
        /// The views default depth is the minimum of width and height.<br />
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// The property cascade chaining set is possible. For example, this (view.Size.Width = 1.0f;) is possible.
        /// </remarks>
        /// <since_tizen> 5 </since_tizen>
        public Size Size
        {
            get
            {
                Size tmp = (Size)GetValue(SizeProperty);
                return new Size(OnSizeChanged, tmp.Width, tmp.Height, tmp.Depth);
            }
            set
            {
                SetValue(SizeProperty, value);
                // Set Specification so when layouts measure this View it matches the value set here.
                // All Views are currently Layouts.
                WidthSpecification = (int)Math.Ceiling(value.Width);
                HeightSpecification = (int)Math.Ceiling(value.Height);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Deprecated in API5; Will be removed in API8. Please use 'Container GetParent() for derived class' instead!
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        [Obsolete("Deprecated in API5; Will be removed in API8. Please use 'Container GetParent() for derived class' instead! " +
            "Like: " +
            "Container parent =  view.GetParent(); " +
            "View view = parent as View;")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new View Parent
        {
            get
            {
                View ret;
                IntPtr cPtr = Interop.Actor.Actor_GetParent(swigCPtr);
                HandleRef CPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
                BaseHandle basehandle = Registry.GetManagedBaseHandleFromNativePtr(CPtr.Handle);

                if (basehandle is Layer layer)
                {
                    ret = new View(Layer.getCPtr(layer).Handle, false);
                    NUILog.Error("This Parent property is deprecated, shoud do not be used");
                }
                else
                {
                    ret = basehandle as View;
                }

                Interop.BaseHandle.delete_BaseHandle(CPtr);
                CPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);

                if (NDalicPINVOKE.SWIGPendingException.Pending)
                    throw NDalicPINVOKE.SWIGPendingException.Retrieve();
                return ret;
            }
        }

        /// <summary>
        /// Gets/Sets whether inherit parent's the layout Direction.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public bool InheritLayoutDirection
        {
            get
            {
                return (bool)GetValue(InheritLayoutDirectionProperty);
            }
            set
            {
                SetValue(InheritLayoutDirectionProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets/Sets the layout Direction.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public ViewLayoutDirectionType LayoutDirection
        {
            get
            {
                return (ViewLayoutDirectionType)GetValue(LayoutDirectionProperty);
            }
            set
            {
                SetValue(LayoutDirectionProperty, value);
                NotifyPropertyChanged();
                _layout?.RequestLayout();
            }
        }

        /// <summary>
        /// Gets or sets the Margin for use in layout.
        /// </summary>
        /// <remarks>
        /// Margin property is supported by Layout algorithms and containers.
        /// Please Set Layout if you want to use Margin property.
        /// The property cascade chaining set is possible. For example, this (view.Margin.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 4 </since_tizen>
        public Extents Margin
        {
            get
            {
                // If View has a Layout then margin is stored in Layout.
                if (Layout != null)
                {
                    return Layout.Margin;
                }
                else
                {
                    // If Layout not set then return margin stored in View.
                    Extents temp = (Extents)GetValue(MarginProperty);
                    return new Extents(OnMarginChanged, temp.Start, temp.End, temp.Top, temp.Bottom);
                }
                // Two return points to prevent creating a zeroed Extent native object before assignment
            }
            set
            {
                if (Layout != null)
                {
                    // Layout set so store Margin in LayoutItem instead of View.
                    // If View stores the Margin too then the Legacy Size Negotiation will
                    // overwrite the position and size values measured in the Layouting.
                    Layout.Margin = value;
                    SetValue(MarginProperty, new Extents(0,0,0,0));
                    _layout?.RequestLayout();
                }
                else
                {
                    SetValue(MarginProperty, value);
                }
                NotifyPropertyChanged();
                _layout?.RequestLayout();
            }
        }

        ///<summary>
        /// The required policy for this dimension, LayoutParamPolicies enum or exact value.
        ///</summary>
        /// <since_tizen> 6 </since_tizen>
        public int WidthSpecification
        {
            get
            {
                return _widthPolicy;
            }
            set
            {
                _widthPolicy = value;
                if (_widthPolicy >= 0)
                {
                    _measureSpecificationWidth = new MeasureSpecification( new LayoutLength(value), MeasureSpecification.ModeType.Exactly );
                    Size2D.Width = _widthPolicy;

                }
                _layout?.RequestLayout();
            }
        }

        ///<summary>
        /// The required policy for this dimension, LayoutParamPolicies enum or exact value.
        ///</summary>
        /// <since_tizen> 6 </since_tizen>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int HeightSpecification
        {
            get
            {
                return _heightPolicy;
            }
            set
            {
                _heightPolicy = value;
                if (_heightPolicy >= 0)
                {
                    _measureSpecificationHeight = new MeasureSpecification( new LayoutLength(value), MeasureSpecification.ModeType.Exactly );
                    Size2D.Height = _heightPolicy;

                }
               _layout?.RequestLayout();
            }
        }

        ///<summary>
        /// Gets the List of transitions for this View.
        ///</summary>
        /// <since_tizen> 6 </since_tizen>
        public Dictionary<TransitionCondition, TransitionList> LayoutTransitions
        {
            get
            {
                if (_layoutTransitions == null)
                {
                    _layoutTransitions = new Dictionary<TransitionCondition, TransitionList>();
                }
                return _layoutTransitions;
            }
        }

        ///<summary>
        /// Set a layout transitions for this View.
        ///</summary>
        /// <remarks>
        /// Use LayoutTransitions to receive a collection of LayoutTransitions set on the View.
        /// </remarks>
        /// <since_tizen> 6 </since_tizen>
        public LayoutTransition LayoutTransition
        {
            set
            {
                if (_layoutTransitions == null)
                {
                    _layoutTransitions = new Dictionary<TransitionCondition, TransitionList>();
                }
                LayoutTransitionsHelper.AddTransitionForCondition(_layoutTransitions,value.Condition,value, true);

                AttachTransitionsToChildren(value);
            }
        }

        /// <summary>
        /// Deprecated in API5; Will be removed in API8. Please use Padding instead.
        /// </summary>
        /// <remarks>
        /// The property cascade chaining set is possible. For example, this (view.DecorationBoundingBox.X = 0.1f;) is possible.
        /// </remarks>
        /// <since_tizen> 4 </since_tizen>
        [Obsolete("Deprecated in API5; Will be removed in API8. Please use Padding instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Extents PaddingEX
        {
            get
            {
                Extents temp = new Extents(0, 0, 0, 0);
                GetProperty(View.Property.PADDING).Get(temp);
                return new Extents(OnPaddingEXChanged, temp.Start, temp.End, temp.Top, temp.Bottom);
            }
            set
            {
                SetProperty(View.Property.PADDING, new Tizen.NUI.PropertyValue(value));
                NotifyPropertyChanged();
                _layout?.RequestLayout();
            }
        }

        /// <since_tizen> 6 </since_tizen>
        /// This will be public opened in tizen_6.0 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Style XamlStyle
        {
            get
            {
                return (Style)GetValue(XamlStyleProperty);
            }
            set
            {
                SetValue(XamlStyleProperty, value);
            }
        }

        /// <summary>
        /// The Color of View. This is an RGBA value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Animatable - This property can be animated using <c>Animation</c> class.
        /// </para>
        /// The property cascade chaining set is possible. For example, this (view.Color.X = 0.1f;) is possible.
        /// </remarks>
        /// This will be public opened in tizen_5.5 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color Color
        {
            get
            {
                Vector4 temp = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                GetProperty(Interop.ActorProperty.Actor_Property_COLOR_get()).Get(temp);
                return new Color(OnColorChanged, temp.R, temp.G, temp.B, temp.A);
            }
            set
            {
                SetColor(value);
            }
        }

        /// <summary>
        /// Set the layout on this View. Replaces any existing Layout.
        /// </summary>
        /// <since_tizen> 6 </since_tizen>
        public LayoutItem Layout
        {
            get
            {
                return _layout;
            }
            set
            {
                // Do nothing if layout provided is already set on this View.
                if (value == _layout)
                {
                    return;
                }

                Log.Info("NUI", "Setting Layout on:" + Name + "\n");
                layoutingDisabled = false;
                layoutSet = true;

                // If new layout being set already has a owner then that owner receives a replacement default layout.
                // First check if the layout to be set already has a owner.
                if (value?.Owner != null)
                {
                    // Previous owner of the layout gets a default layout as a replacement.
                    value.Owner.Layout = new LayoutGroup();

                    // Copy Margin and Padding to replacement LayoutGroup.
                    value.Owner.Layout.Margin = value.Margin;
                    value.Owner.Layout.Padding = value.Padding;
                }

                // Copy Margin and Padding to new layout being set or restore padding and margin back to
                // View if no replacement. Previously margin and padding values would have been moved from
                // the View to the layout.
                if (_layout != null ) // Existing layout
                {
                    if (value != null)
                    {
                        // Existing layout being replaced so copy over margin and padding values.
                        value.Margin = _layout.Margin;
                        value.Padding = _layout.Padding;
                    }
                    else
                    {
                      // Layout not being replaced so restore margin and padding to View.
                      SetValue(MarginProperty, _layout.Margin);
                      SetValue(PaddingProperty, _layout.Padding);
                      NotifyPropertyChanged();
                    }
                }
                else
                {
                    // First Layout to be added to the View hence copy

                    // Do not try to set Margins or Padding on a null Layout (when a layout is being removed from a View)
                    if (value !=null)
                    {
                        if (Margin.Top != 0 || Margin.Bottom !=0 || Margin.Start !=0 || Margin.End != 0)
                        {
                            // If View already has a margin set then store it in Layout instead.
                            value.Margin = Margin;
                            SetValue(MarginProperty, new Extents(0,0,0,0));
                            NotifyPropertyChanged();
                        }

                        if (Padding.Top != 0 || Padding.Bottom !=0 || Padding.Start !=0 || Padding.End != 0)
                        {
                            // If View already has a padding set then store it in Layout instead.
                            value.Padding = Padding;
                            SetValue(PaddingProperty, new Extents(0,0,0,0));
                            NotifyPropertyChanged();
                        }
                    }
                }

                // Remove existing layout from it's parent layout group.
                _layout?.Unparent();

                // Set layout to this view
                SetLayout(value);
            }
        }

        /// <summary>
        /// The weight of the View, used to share available space in a layout with siblings.
        /// </summary>
        /// <since_tizen> 6 </since_tizen>
        public float Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                _layout?.RequestLayout();
            }
        }

        /// <summary>
        ///  Whether to load the BackgroundImage synchronously.
        ///  If not specified, the default is false, i.e. the BackgroundImage is loaded asynchronously.
        ///  Note: For Normal Quad images only.
        /// </summary>
        /// This will be public opened in tizen_5.5 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool BackgroundImageSynchronosLoading
        {
            get
            {
                return _backgroundImageSynchronosLoading;
            }
            set
            {
                _backgroundImageSynchronosLoading = value;
                string bgUrl = "";
                int visualType = 0;
                Background.Find(Visual.Property.Type)?.Get(out visualType);
                if (visualType == (int)Visual.Type.Image)
                {
                    Background.Find(ImageVisualProperty.URL)?.Get(out bgUrl);
                }

                if (bgUrl.Length != 0)
                {
                    PropertyMap bgMap = this.Background;
                    bgMap.Add("synchronousLoading", new PropertyValue(_backgroundImageSynchronosLoading));
                    Background = bgMap;
                }
            }
        }

        /// This will be public opened in tizen_6.0 after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Vector2 UpdateSizeHint
        {
            get
            {
                return (Vector2)GetValue(UpdateSizeHintProperty);
            }
            set
            {
                SetValue(UpdateSizeHintProperty, value);
                NotifyPropertyChanged();
            }
        }

        /// This will be public opened in tizen_next after ACR done. Before ACR, need to be hidden as inhouse API.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string[] TransitionNames
        {
            get
            {
                return transitionNames;
            }
            set
            {
                transitionNames = value;
                LoadTransitions();
            }
        }
    }
}
