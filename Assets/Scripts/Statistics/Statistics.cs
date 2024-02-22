using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public record Statistics(
    float meanSteps
    , float meanEvacuationTime
    , float medianSteps
    , float medianEvacuationTime
    , int numberOfEvacuees
    , int numberOfNonEvacuees) 
{
    
    public float meanSteps { get; set; } = meanSteps;
    public float meanEvacuationTime { get; set; } = meanEvacuationTime;
    public float medianSteps { get; set; } = medianSteps;
    public float medianEvacuationTime { get; set; } = medianEvacuationTime;
    public int numberOfEvacuees { get; set; } = numberOfEvacuees;
    public int numberOfNonEvacuees { get; set; } = numberOfNonEvacuees;
    
    public static bool bernoulli(float successProbability) {
        if (!(successProbability < 0.0) && !(successProbability > 1.0)) {
            return Random.value < successProbability;
        } else {
            Debug.LogError("bernoulli: probability " + successProbability + "must be in [0.0, 1.0]");
            return false;
        }
    }
    
    public static T Discrete<T>(IEnumerable<T> collection, Func<T, float> desirability)
    {
        float sum = collection.Sum(item => desirability(item));
        if (sum <= 0.0)
        {
            throw new ArgumentException("Discrete: sum of desirabilities must be larger than 0");
        }
        else
        {
            float choose = Random.value * sum;
            sum = 0.0f;
            foreach (var element in collection)
            {
                sum += desirability(element);
                if (sum > choose)
                {
                    return element;
                }
            }
            Debug.Log(sum);
            throw new InvalidOperationException("Should not get here if the desirabilities sum to more than 0");
        }
    }
    
    public static float Mean(int[] data) {
        if (data == null || data.Length == 0) {
            throw new ArgumentException("mean: data cannot be empty");
        } else {
            int sum = 0;
            int length = data.Length;

            for(int i = 0; i < length; i++) {
                sum += data[i];
            }

            return (float)sum / length;
        }
    }
    
    public static float Mean(float[] data) {
        if (data == null || data.Length == 0) {
            throw new ArgumentException("mean: data cannot be empty");
        } else {
            float sum = 0.0f;
            int length = data.Length;

            for(int i = 0; i < length; i++) {
                sum += data[i];
            }

            return sum / length;
        }
    }
    
    public static float Median(int[] data) {
        if (data == null || data.Length == 0) {
            throw new ArgumentException("median: data cannot be empty");
        } else {
            //make sure the list is sorted, but use a new array
            int[] sortedPNumbers = (int[])data.Clone();
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            float median = (size % 2 != 0) ? sortedPNumbers[mid] : (float)(sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }
    
    public static float Median(float[] data) {
        if (data == null || data.Length == 0) {
            throw new ArgumentException("median: data cannot be empty");
        } else {
            //make sure the list is sorted, but use a new array
            float[] sortedPNumbers = (float[])data.Clone();
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            float median = (size % 2 != 0) ? sortedPNumbers[mid] : (sortedPNumbers[mid] +sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }
}