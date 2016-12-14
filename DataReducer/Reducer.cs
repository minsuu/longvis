using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReducer
{
    public static class Reducer
    {
        /// <summary>
        /// MinMax로 reduction을 수행하는 부분. timestamp가 같은 경우의 처리를 하고 recursion을 수행한다.
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="result"></param>
        public static void reduce_MinMax(List<long> px, List<double> py, out long[] result)
        {
            result = new long[px.Count];
            index = 0; len = px.Count;
            reduce_MinMax_rec(px.First(), px.Last()+1, 0, px, py, ref result);
        }
        
        // woking progress. 0~100.
        public static double workProgress {
            get
            {
                return (double)index / len * 100;
            }
        }

        /// <summary>
        /// reduce된 결과를 담는 entry이다. 시작, 끝, 최대, 최소점의 위치를 담고있다.
        /// </summary>
        private struct reduce_Entry
        {
            public int begin, end, maxp, minp;
            public reduce_Entry(int _begin, int _end, int _maxp, int _minp)
            {
                begin = _begin; end = _end; maxp = _maxp; minp = _minp;
            }
        }
        private static reduce_Entry reduce_EntryNull = new reduce_Entry(-1,-1,-1,-1);
        private static int index;
        private static int len;

        /// <summary>
        ///     [leftBound, rightBound]의 구간에서 index로부터 출발해서 result에다 값을 할당한다
        /// </summary>
        /// <param name="leftBound">좌측 한계</param>
        /// <param name="rightBound">우측 한계</param>
        /// <param name="index">시작 index</param>
        /// <param name="px">x좌표 값들</param>
        /// <param name="py">y좌표 값들</param>
        /// <param name="result">결과 bitmask</param>
        private static reduce_Entry reduce_MinMax_rec(long leftBound, long rightBound, int depth, List<long> px, List<double> py, ref long[] result)
        {
            // atomic한 case로, 해당 범위 안에 있는 data가 단 하나일 경우이다.
            if (index+1 == px.Count || rightBound <= px[index+1]) {
                result[index] = 1 << depth;
                reduce_Entry ret = new reduce_Entry(index,index,index,index);
                index++;
                return ret;
            }
            
            // 범위를 반으로 나눠, 왼쪽과 오른쪽의 entry를 각각 먼저 구해둔다.
            long mid = leftBound/2 + rightBound/2 + (leftBound%2 + rightBound%2)/2;
            reduce_Entry left = reduce_MinMax_rec(leftBound, mid, depth+1, px, py, ref result);
            reduce_Entry right = reduce_MinMax_rec(mid, rightBound, depth+1, px, py, ref result);

            // 네 가지 경우 각각 중요도가 높은 대상에 대해 더 낮은 Q값을 할당해준다.
            int maxp = (py[left.maxp] < py[right.maxp]) ? right.maxp : left.maxp;
            int minp = (py[left.minp] < py[right.minp]) ? left.minp : right.minp;
            result[left.begin] = 1 << depth;
            result[right.end] = 1 << depth;
            result[maxp] = 1 << depth;
            result[minp] = 1 << depth;

            return new reduce_Entry(left.begin, right.end, maxp, minp);
        }
    }
}
