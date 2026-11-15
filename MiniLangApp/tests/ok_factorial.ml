// факторіал 5
let n = 5;
let f = 1;
while (n > 1) {
    f = f * n;
    n = n - 1;
}
print("5! = " + f);
if (f == 120) print("OK"); else print("FAIL");