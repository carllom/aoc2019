# Problem notes

10000 reps of 650 = 6500000

* It's really a matter of which number to add and which to subtract
* 50% of numbers (3 250 000) are involved in calculation of each digit. In total 6500000 * 3250000 per phase.. too many to be done naively. It takes ~1s to do 650 numbers => at least 30h to do the entire calculation.
* Where is the repetition? think of it like expanding fields:

```text
   00000000001111111111222222222233333333334444444444555555555
   01234567890123456789012345678901234567890123456789012345678
0  + - + - + - + - + - + - + - + - + - + - + - + - + - + - + -
1   ++  --  ++  --  ++  --  ++  --  ++  --  ++  --  ++  --  ++
2    +++   ---   +++   ---   +++   ---   +++   ---   +++   ---
3     ++++    ----    ++++    ----    ++++    ----    ++++    
4      +++++     -----     +++++     -----     +++++     -----
5       ++++++      ------      ++++++      ------      ++++++
6        +++++++       -------       +++++++       -------
7         ++++++++        --------        ++++++++        ----
8          +++++++++         ---------         +++++++++      
9           ++++++++++          ----------          ++++++++++
```

* Try to find repetitions in multiples of source length
* Only calculate dependencies of digit n

To calculate digit 0 you need to calculate every other digit. Thats 3250000 - still a lot

length*n / convlen = whole number

650*2 /  4 = 325. Digit 0 repeats after  2 source repetitions. Multiply that by 10000/2 = 5000
650*4 /  8 = 325. Digit 1 repeats after  4 source repetitions. Multiply that by 10000/4 = 2500
650*6 / 12 = 325. Digit 2 repeats after  6 source repetitions. Multiply that by 10000/6 = 1666.66 ~
650*8 / 16 = 325. Digit 3 repeats after  8 source repetitions. Multiply that by 10000/8 = 1250 
650*10/ 20 = 325. Digit 4 repeats after  2 source repetitions. Multiply that by 10000/2 = 5000
650*12/ 24 = 325. Digit 5 repeats after 12 source repetitions. Multiply that by 10000/12 = 833.33 ~
650*14/ 28 = 325. Digit 6 repeats after 14 source repetitions. Multiply that by 10000/14 = 714.285 ~
650*16/ 32 = 325. Digit 7 repeats after 16 source repetitions. Multiply that by 10000/16 = 625
trivial case seems to be 2*i repetitions
650*(650*2) = 845000

* Can we iterate over a single number given the repetition length as input? For position 0 it is 1300 numbers (325 repetitions of 4 )

The thing is, we don't know the repetition length in iteration 2+. Iteration 1 is OK, since we have a known input.

The only way we can reduce the number of operations is if we can copy half-calculated values. 
There does not seem to be a good way to work "vertically", since we always have a slant and 

```text
   00000000001111111111222222222233333333334444444444555555555
   01234567890123456789012345678901234567890123456789012345678
0  + - + - + - + - + - + - + - + - + - + - + - + - + - + - + -
1   ++  --  ++  --  ++  --  ++  --  ++  --  ++  --  ++  --  ++
2    +++   ---   +++   ---   +++   ---   +++   ---   +++   ---
3     ++++    ----    ++++    ----    ++++    ----    ++++    
4      ++++y     -----     +++++     -----     +++++     -----
5       +++xyy      ------      ++++++      ------      ++++++
6        ++xxxyy       -------       +++++++       -------
7         +xxxxxyy        --------        ++++++++        ----
8          xxxxxxxyy         ---------         +++++++++      
9           ++++++++++          ----------          ++++++++++
```

..or perhaps we could subtract and add to reduce the operations

```text
   00000000001111111111222222222233333333334444444444555555555
   01234567890123456789012345678901234567890123456789012345678
0  ! - + - + - + - + - + - + - + - + - + - + - + - + - + - + -
1  _!!  --  ++  --  ++  --  ++  --  ++  --  ++  --  ++  --  ++
2   _+!!   ---   +++   ---   +++   ---   +++   ---   +++   ---
3    _++!!    ----    ++++    ----    ++++    ----    ++++    
4     _+++!!  !!!-____     +++++     -----     +++++     -----
5      _++++!!   !!!--____      ++++++      ------      ++++++
6       _+++++!!    !!!---____       +++++++       -------
7        _++++++!!     !!!----____        ++++++++        ----
8         _+++++++!!      !!!-----____         +++++++++      
9          _++++++++!!       !!!------____          ++++++++++
```

See above, when the overlap is larger than the differencing operations, we could benefit from copied values. Question is, how much?
First section has 3 differencing operations (remove 1, add 2). Second section has 7 differencing operations (remove 3, add 4), third has 11 (remove 5, add 6), and so on.
It does not feel like a stable method though, especially when you move further to the right.

Or is it? what happens when you move backwards through this?

```text
   000000000011111  11111222222222233333333334444444444555555555
   012345678901234  56789012345678901234567890123456789012345678
0  + - + - + - + -   + - + - + - + - + - + - + - + - + - + - + -
1   ++  --  ++  --    ++  --  ++  --  ++  --  ++  --  ++  --  ++
2    +++   ---   +  ++   ---   +++   ---   +++   ---   +++   ---

3     ++++    ----      ++++    ----    ++++    ----    ++++    
4      +++++     -  ----     +++++     -----     +++++     -----

5       ++++++        ------      ++++++      ------      ++++++
6        +++++++         -------       +++++++       -------
7         ++++++++          --------        ++++++++        ----
8          +++++++  ++         ---------         +++++++++      
9           ++++++  ++++          ----------          ++++++++++
10           +++++  ++++++           -----------           +++++
11            ++++  ++++++++            ------------            
12             +++  ++++++++++             -------------        
13              ++  ++++++++++++              --------------    
14               +  ++++++++++++++               ---------------
```

* The nth number just has the nth input (first=n, last = 2*n)
* The n-1th number adds the n-1th number to the previous and so on..
* ..until 6 when last is 2 below n (12)
* So, if we do a special operation for 14 downto 7, (or even 5 if we subtract as well)
* ..and then also does the same for the next negative section starting at 4 and ending at 3
* ..and perhaps again for the next positive range we could effectively calculate a major part of the stream
* If we just do the positive bit, we can calculate 2/3 of the array.
* The rest can be calculated the ordinary way


```text
   0 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2  222233333333334444444444555555555
   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5  678901234567890123456789012345678
0  +   -   +   -   +   -   +   -   +   -   +   -   +    - + - + - + - + - + - + - + - + -
1 f3 + +     - -     + +     - -     + +     - -     +  +  --  ++  --  ++  --  ++  --  ++
2  .2. + +3+ . .3. - -3- . .3. + +3+ . .3. - - -        +++   ---   +++   ---   +++   ---
3   f2   + + + +         - - - -         + + + +         ----    ++++    ----    ++++    
4  . .4. . + + +5+ + . . .5. . - - -5- - . . .5. . + +  +++     -----     +++++     -----
5            + + + + + +             - - - - - -           ++++++      ------      ++++++
6       f1     + + + + + + +               - - - - - -  -       +++++++       -------
7  . . . .7. . . + + + +8+ + + + . . . .8. . . . - - -  -----        ++++++++        ----
8                  + + + + + + + + +                    ---------         +++++++++      
9                    + + + + + + + + + +                   ----------          ++++++++++
10                     + + + + + + + + + + +                  -----------           +++++
11                       + + + + + + + + + + + +                 ------------            
12                         + + + + + + + + + + + + +                -------------        
13                           + + + + + + + + + + + + +  +              --------------    
14                             + + + + + + + + + + + +  +++               ---------------
15                               + + + + + + + + + + +  +++++                ------------
16                                 + + + + + + + + + +  +++++++                 ---------
17                                   + + + + + + + + +  +++++++++                  ------
18                                     + + + + + + + +  +++++++++++                   ---
19                                       + + + + + + +  +++++++++++++                    
20                                         + + + + + +  +++++++++++++++                  
21                                           + + + + +  +++++++++++++++++                
22                                             + + + +  +++++++++++++++++++              
23                                               + + +  +++++++++++++++++++++            
24                 f0                              + +  +++++++++++++++++++++++          
25 . . . . . . . . . . . 25. . . . . . . . . . . . . +  +++++++++++++++++++++++++        
```

first appearance of fan
y + (f*2*(y+1)) <= 25
25 (1,2) 25 + (0*2*(26)) = 25
 7 (3,4) 7 + (1*2*8) = 23, (next is 8+18=26 > 25)
 4 (5,6) 4 + (2*2*5) = 24, (next is 4+24=28 > 25)
 2 (7,8) 2 + (3*2*3) = 20, (next is 3+24=27 > 25)
 1 (9,10)1 + (4*2*2) = 17, (next is 2+24=26 > 25)
 1 (11,12)1+ (5*2*2) = 21, (next is 2+30=32 > 25)
 1 (13,14)1+ (6*2*2) = 25, (next is 2+36=38 > 25)
 0 (15,16)0+ (7*2*1) = 14, (next is 1+28=29 > 25)
and so on...

y <= len/(f*2+1)-1
works for
fan y(0-based index)
0 => 26-1 = 25
1 => 26/3-1 = 7,66 (floor) = 7
2 => 26/5-1 = 4,2 (floor) = 4
3 => 26/7-1 = 2,7 (floor) = 2

outindex crossover to disjunct (for edgedelta)
1-0 (1,2)
3-2 (3,4)
5-4 (5,6)
7-6 (7,8)
9-8 (9,10)

When a fan crosses over to have disjunct segments, just sum them up without doing diffs.

1 2 3 4 5 6 7 8 input
4 8 2 2 6 1 5 8 correct

1 1 8 2 6 1 5 8
   +3+4+5+6+7+8
   -D-8

597175139489
526550430179
526950430179
+ - + - + - 0
 ++  --  ++ 1
  +++   --- 2
   ++++    -3 25+1 -3-9 = 14
    +++++   4 7+5+1+3+9 = 25


0  5 = 5
1 16 = 15+9-7-1
2 15 = 14+7-1-5
3 14 = 25+1-9-3 <= -9
4 25 = 30+7-8-4
5 30 = 34+5-9
6 34 = 33+1
7 33 = 30+3
8 30 = 21+9
9 21 = 17+4
A 17 = 9+8
B  9 = 9

