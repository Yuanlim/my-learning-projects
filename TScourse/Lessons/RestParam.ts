// Rest Parameters:
// The rest of the parameters thats being
// pass in organize into one array
// even it is not an array pass

const total = (a: number, ...nums: number[]): number => {
    return a + nums.reduce((prev, curr) => prev + curr)
};

                //a /...nums/
console.log(total(1, 2, 3, 4));
