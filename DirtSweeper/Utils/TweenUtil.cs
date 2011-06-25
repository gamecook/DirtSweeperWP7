using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GameCook.DirtSweeper.Utils
{
    public class TweenUtil
    {
        public static Storyboard AnimateInStageLeft(FrameworkElement target, double delay = 0, bool autoStart = true)
        {
            double from = -800;

            Storyboard sb;

            var trans = new TranslateTransform {X = from};
            target.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);

            var db = new DoubleAnimation();
            db.To = 0;
            db.From = from;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(.5);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("X"));

            if (autoStart)
                sb.Begin();

            return sb;
        }

        public static Storyboard AnimateOutStageLeft(FrameworkElement target, double delay = 0, bool autoStart = true)
        {
            double from = -800;

            Storyboard sb;

            var trans = new TranslateTransform {X = from};
            target.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);

            var db = new DoubleAnimation();
            db.To = from;
            db.From = 0;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(.5);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("X"));

            if (autoStart)
                sb.Begin();

            return sb;
        }

        public static Storyboard AnimateInStageRight(FrameworkElement target, double delay = 0, bool autoStart = true)
        {
            double from = target.ActualWidth + 800;

            Storyboard sb;

            var trans = new TranslateTransform {X = from};
            target.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);

            var db = new DoubleAnimation();
            db.To = 0;
            db.From = from;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(.5);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("X"));

            if (autoStart)
                sb.Begin();

            return sb;
        }

        public static Storyboard AnimateInStageBottom(FrameworkElement target, double delay = 0, bool autoStart = true)
        {
            //TODO this shouldn't be hardcoded but then again all phones are this screen size.
            double from = 820;

            Storyboard sb;

            var trans = new TranslateTransform {Y = from};
            target.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);

            var db = new DoubleAnimation();
            db.To = 0;
            db.From = from;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(.5);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("Y"));

            if (autoStart)
                sb.Begin();

            return sb;
        }

        public static Storyboard AutoScale(FrameworkElement target, double range = 0, bool autoStart = true)
        {
            var sb = new Storyboard();

            var scaleTransform = new ScaleTransform();
            target.RenderTransform = scaleTransform;

            scaleTransform.CenterX = target.Width*.5;

            scaleTransform.CenterY = target.Height*.5;
            var doubleAnimation = new DoubleAnimation();

            sb.Children.Add(doubleAnimation);

            doubleAnimation.Duration = TimeSpan.FromSeconds(0.4);
            doubleAnimation.From = 1.0;
            doubleAnimation.To = range;

            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTarget(doubleAnimation, scaleTransform);

            sb.AutoReverse = true;
            sb.RepeatBehavior = RepeatBehavior.Forever;

            if (autoStart)
                sb.Begin();

            return sb;
        }

        public static Storyboard AnimateInStageTop(FrameworkElement target, double delay = 0, bool autoStart = true)
        {
            //TODO this shouldn't be hardcoded but then again all phones are this screen size.
            double from = -target.ActualHeight;

            Storyboard sb;

            var trans = new TranslateTransform { Y = from };
            target.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(delay);

            var db = new DoubleAnimation();
            db.To = 0;
            db.From = from;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(.5);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("Y"));

            if (autoStart)
                sb.Begin();

            return sb;
        }
    }
}