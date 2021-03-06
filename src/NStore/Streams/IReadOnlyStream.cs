﻿using System;
using System.Threading;
using System.Threading.Tasks;
using NStore.Raw;

namespace NStore.Streams
{
    public interface IReadOnlyStream
    {
        /// <summary>
        /// Read from stream
        /// </summary>
        /// <param name="partitionObserver"></param>
        /// <param name="fromIndexInclusive"></param>
        /// <param name="toIndexInclusive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Read(
            IPartitionObserver partitionObserver, 
            int fromIndexInclusive = 0, 
            int toIndexInclusive = Int32.MaxValue, 
            CancellationToken cancellationToken = default(CancellationToken)
        );
    }
}