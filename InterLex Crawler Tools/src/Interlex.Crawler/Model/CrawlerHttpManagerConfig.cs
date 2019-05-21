namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Config for the <see cref="Manager.HttpCrawlerManager"/> class
    /// </summary>
    public class CrawlerHttpManagerConfig
    {
        /// <summary>
        /// Default config. <see cref="MaxDegreeOfParallel"/> = <see cref="Environment.ProcessorCount"/>
        /// </summary>
        public static readonly CrawlerHttpManagerConfig Default = new CrawlerHttpManagerConfig(Environment.ProcessorCount);

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="maxDegreeOfParallel">Max degree of parallel</param>
        public CrawlerHttpManagerConfig(int maxDegreeOfParallel)
        {
            this.MaxDegreeOfParallel = maxDegreeOfParallel;
        }

        /// <summary>
        /// Max degree of parallel
        /// </summary>
        public int MaxDegreeOfParallel { get; }
    }
}
