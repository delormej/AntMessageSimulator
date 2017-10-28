#import pandas as pd
import numpy as np

d = {"sell": [
           {
               "Rate": 0.001425,
               "Quantity": 537.27713514
           },
           {
               "Rate": 0.00142853,
               "Quantity": 6.59174681
           }
]}

#df = pd.DataFrame(d['sell'])
#print (df)

#df.plot(x='Quantity', y='Rate')
x = np.array([(1,2.,'Hello'), (2,3.,"World")], dtype=[('foo', 'i4'),('bar', 'f4'), ('baz', 'S10')])
    
print(x[1])