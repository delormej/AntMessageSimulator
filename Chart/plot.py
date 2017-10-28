import os
import sys
import json
from pprint import pprint
import matplotlib.pyplot as plt

def open_ride(file_name):
    with open(file_name) as data_file:
        data = json.load(data_file)

    print(data["data"][0])
    return data["data"]

def plot_ride(data):
    plt.plot(range(0, 2), data)
    plt.show()

#
# Main entry point to parse a file.
#        
def main(file_name):
    data = open_ride(file_name)
    plot_ride(data)
        
if __name__ == "__main__":
    main(sys.argv[1])
    