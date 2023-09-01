int arrayLength = Convert.ToInt32(Console.ReadLine());

int[] resultArray = new int[arrayLength];

Dictionary<int, List<int>> prohibitedIntervalBoundaryValues = new Dictionary<int, List<int>>();

List<int> indexesOfUnknownValues = Enumerable.Range(1, arrayLength).ToList(); 

int leftIntervalBorder = 1;
int rightIntervalBorder;
string requestString;

while (leftIntervalBorder < arrayLength + 1)
{
    rightIntervalBorder = leftIntervalBorder;

    if (prohibitedIntervalBoundaryValues.ContainsKey(leftIntervalBorder))
    {
        if (prohibitedIntervalBoundaryValues[leftIntervalBorder].Contains(rightIntervalBorder))
        {
            leftIntervalBorder++;
            continue;
        }
    }

    SendRequest(leftIntervalBorder, rightIntervalBorder);

    resultArray[leftIntervalBorder - 1] = (int) HandleResponse();

    indexesOfUnknownValues.Remove(leftIntervalBorder);
    leftIntervalBorder++;
}

while (indexesOfUnknownValues.Count > 0)
{
    List<int> currentIndexesOfUnknownValues = new List<int>();
    currentIndexesOfUnknownValues.AddRange(indexesOfUnknownValues);

    foreach (var index in currentIndexesOfUnknownValues)
    {
        List<int> allLesserIndexes = Enumerable.Range(1, index-1).ToList();

        List<int> allowedIndexesForLeftBorder = allLesserIndexes.Except(prohibitedIntervalBoundaryValues[index]).ToList();

        if (allowedIndexesForLeftBorder.Exists(x => x < index && !indexesOfUnknownValues.Contains(x)))
        {
            int nearestLesserIndex = allowedIndexesForLeftBorder.FindLast(x => x < index && !indexesOfUnknownValues.Contains(x));

            if (!indexesOfUnknownValues.Exists(x => nearestLesserIndex < x && x < index))
            {
                leftIntervalBorder = nearestLesserIndex;
                rightIntervalBorder = index;

                SendRequest(leftIntervalBorder, rightIntervalBorder);

                resultArray[rightIntervalBorder - 1] = GetElementValueByKnownIntervalSum(leftIntervalBorder, rightIntervalBorder);

                indexesOfUnknownValues.Remove(rightIntervalBorder);

                continue;
            }
        }

        int countOfBiggerIndexes = arrayLength - index;

        List<int> allBiggerIndexes = Enumerable.Range(index + 1, countOfBiggerIndexes).ToList();

        List<int> allowedIndexesForRightBorder = allBiggerIndexes.Except(prohibitedIntervalBoundaryValues[index]).ToList();

        if (allowedIndexesForRightBorder.Exists(x => index < x && !indexesOfUnknownValues.Contains(x)))
        {
            int nearestBiggerIndex = allowedIndexesForRightBorder
                .Find(x => index < x && !indexesOfUnknownValues.Contains(x));

            if (!indexesOfUnknownValues.Exists(x => index < x && x < nearestBiggerIndex))
            {
                leftIntervalBorder = index;
                rightIntervalBorder = nearestBiggerIndex;

                SendRequest(leftIntervalBorder, rightIntervalBorder);

                resultArray[leftIntervalBorder - 1] = GetElementValueByKnownIntervalSum(leftIntervalBorder, rightIntervalBorder);

                indexesOfUnknownValues.Remove(leftIntervalBorder);
            }
        }
    }
}

void SendRequest(int leftBorder, int rightBorder)
{
    requestString = $"? {leftIntervalBorder} {rightIntervalBorder}";
    Console.WriteLine(requestString);
}

long HandleResponse()
{
    string[] response = Console.ReadLine().Split(" ");
    long intervalSum = Convert.ToInt64(response[0]);
    int firstProhibitedBorder = Convert.ToInt32(response[1]);
    int secondProhibitedBorder = Convert.ToInt32(response[2]);

    if (!prohibitedIntervalBoundaryValues.ContainsKey(firstProhibitedBorder))
    {
        prohibitedIntervalBoundaryValues.Add(firstProhibitedBorder, new List<int>());
    }

    if (!prohibitedIntervalBoundaryValues.ContainsKey(secondProhibitedBorder))
    {
        prohibitedIntervalBoundaryValues.Add(secondProhibitedBorder, new List<int>());
    }

    if (firstProhibitedBorder != secondProhibitedBorder)
    {
        prohibitedIntervalBoundaryValues[firstProhibitedBorder].Add(secondProhibitedBorder);
        prohibitedIntervalBoundaryValues[secondProhibitedBorder].Add(firstProhibitedBorder);
    }
    else
    {
        prohibitedIntervalBoundaryValues[firstProhibitedBorder].Add(secondProhibitedBorder);
    }

    return intervalSum;
}

int GetElementValueByKnownIntervalSum(int leftBorder, int rightBorder)
{
    long intervalSum = HandleResponse();
    long sumOfKnownElements = 0;

    for (int i = leftIntervalBorder - 1; i < rightIntervalBorder; i++)
    {
        sumOfKnownElements += resultArray[i];
    }

    return (int)(intervalSum - sumOfKnownElements);
}

Console.WriteLine("!");
for (int i = 0; i < arrayLength - 1; i++)
{
    Console.Write(resultArray[i] + " ");
}
Console.WriteLine(resultArray[arrayLength - 1]);

Console.WriteLine("Для закрытия приложения нажмите любую кнопку");
Console.ReadLine();
